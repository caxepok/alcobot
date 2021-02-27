using alcobot.service.Infrastructure;
using alcobot.service.Models;
using alcobot.service.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
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

        public async Task CreateOrUpdateChatAsync(long id, string title)
        {
            using var context = Context;
            if (!_chats.TryGetValue(id, out var chat))
            {
                chat = context.Chats.SingleOrDefault(_ => _.Id == id);
                if (chat == null)
                {
                    chat = new Chat() { Id = id, Name = title };
                    await context.AddAsync(chat);
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Created chat, id: {id}, name: {title}");
                }
                _chats.AddOrUpdate(id, chat, (id, chat) => chat);
            }
            // todo: сверять название чата, и если надо обновлять его название
        }

        public async Task<string> ProcessMessageAsync(long chatId, long userId, string username, string message)
        {
            _logger.LogInformation($"processing message: {message}");   // todo: пока для отладки, потом убрать
            var now = DateTimeOffset.Now;
            // parse drinks           
            var drinks = _messageParserService.ParseMessageToDrinks(message);
            if (!drinks.Any())
                return null;
            // add user if not exist
            using var context = Context;
            if (!_drinkers.TryGetValue(userId, out var drinker))
            {
                drinker = context.Drinkers.SingleOrDefault(_ => _.Id == userId);
                if (drinker == null)
                {
                    drinker = new Drinker() { ChatId = chatId, Id = userId, Username = username };
                    context.Drinkers.Add(drinker);
                    _logger.LogInformation($"created user, id: {userId}, username: {username}");
                }
                _drinkers.AddOrUpdate(userId, drinker, (userId, drinker) => drinker);
            }
            // add drinks
            foreach (var drink in drinks)
            {
                drink.UserId = userId;
                drink.Timestamp = now;
            }
            await context.AddRangeAsync(drinks);
            await context.SaveChangesAsync();

            // todo: добавить сюда емоджи 🍺
            return $"Записал: {String.Join(',', drinks.Select(_ => $"{_.DrinkType}: {_.Volume}мл"))}";
        }

        private AlcoDBContext Context =>
            _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AlcoDBContext>();
    }
}
