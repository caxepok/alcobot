using alcobot.service.Infrastructure;
using alcobot.service.Models;
using alcobot.service.Services.Interfaces;
using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Services
{
    /// <inheritdoc cref="IAlcoCounterService"/>
    public class AlcoCounterService : IAlcoCounterService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageParserService _messageParserService;

        private ConcurrentDictionary<long, Chat> _chats = new ConcurrentDictionary<long, Chat>();
        private ConcurrentDictionary<long, Drinker> _drinkers = new ConcurrentDictionary<long, Drinker>();

        public AlcoCounterService(ILogger<AlcoCounterService> logger, IServiceProvider serviceProvider, IMessageParserService messageParserService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _messageParserService = messageParserService;
        }

        public Task CreateOrUpdateChatAsync(long id, string title) =>
            GetChatAsync(id, title);

        public async Task<string> ProcessMessageAsync(long chatId, long userId, string username, string message)
        {
            _logger.LogInformation("processing message: {message}", message);
            var now = DateTimeOffset.Now;
            // parse drinks           
            var drinks = _messageParserService.ParseMessageToDrinks(message);

            // add user if not exist
            var drinker = await GetDrinkerAsync(userId, username);
            var chat = await GetChatAsync(chatId, username);
            using var context = GetContext();

            if (!drinks.Any())
            {
                context.Messages.Add(new Message() { ChatId = chatId, UserId = userId, Text = message, Timestamp = now, IsRecognized = false });
                await context.SaveChangesAsync();
                return "Прости, не очень разобрался что ты там написал, давай чётче, я же бот. Хочешь примеров? Набери /help.";
            }
            // add drinks
            foreach (var drink in drinks)
            {
                drink.UserId = userId;
                drink.ChatId = chatId;
                drink.Timestamp = now;
            }
            await context.AddRangeAsync(drinks);
            context.Messages.Add(new Message() { ChatId = chatId, UserId = userId, Text = message, Timestamp = now, IsRecognized = true });
            await context.SaveChangesAsync();

            // todo: добавить сюда емоджи 🍺
            return $"Записал: {String.Join(',', drinks.Select(_ => _messageParserService.DescribeDrink(_)))}";
        }

        public async Task<byte[]> ExportAsync(long chatId, long userId)
        {
            using var context = GetContext();
            var drinks = context.Drinks.Where(_ => _.UserId == userId);
            MemoryStream memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(drinks);
            await writer.FlushAsync();
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Возвращает выпивающего из кеша\базы или создаёт нового
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="username">имя пользователя</param>
        private async Task<Drinker> GetDrinkerAsync(long userId, string username)
        {
            if (_drinkers.TryGetValue(userId, out var drinker))
                return drinker;

            using var context = GetContext();
            drinker = context.Drinkers.SingleOrDefault(_ => _.Id == userId);
            if (drinker != null)
                // todo: тут может добавить актуализацию названия выпивающего
                return drinker;

            drinker = new Drinker() { Id = userId, Username = username };
            context.Drinkers.Add(drinker);
            await context.SaveChangesAsync();

            _logger.LogInformation($"created user, id: {userId}, username: {username}");
            _drinkers.AddOrUpdate(userId, drinker, (userId, drinker) => drinker);
            return drinker;
        }

        /// <summary>
        /// Возвращает чат из кеша\базы или создаёт новый
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <param name="title">заголовок чата</param>
        private async Task<Chat> GetChatAsync(long chatId, string title)
        {
            if (_chats.TryGetValue(chatId, out var chat))
                return chat;

            using var context = GetContext();
            chat = context.Chats.SingleOrDefault(_ => _.Id == chatId);
            if (chat != null)
                // todo: тут может добавить актуализацию названия группы
                return chat;

            chat = new Chat() { Id = chatId, Name = title };
            await context.AddAsync(chat);
            await context.SaveChangesAsync();
            _logger.LogInformation($"Created chat, id: {chatId}, name: {title}");
            _chats.AddOrUpdate(chatId, chat, (chatId, chat) => chat);
            return chat;
        }

        /// <summary>
        /// Возвращает новый контекст для подключения к базе
        /// </summary>
        private AlcoDBContext GetContext() =>
            _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AlcoDBContext>();
    }
}
