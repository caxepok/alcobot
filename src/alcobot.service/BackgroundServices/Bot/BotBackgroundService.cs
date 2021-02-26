using alcobot.service.Infrastructure;
using alcobot.service.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace alcobot.service.BackgroundServices.Bot
{
    public class BotBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _botClient;
        private readonly IAlcoCounterService _alcoCounterService;

        public BotBackgroundService(ILogger<BotBackgroundService> logger, IOptions<AppSettings> options, IAlcoCounterService alcoCounterService)
        {
            _logger = logger;
            _botClient = new TelegramBotClient(options.Value.BotApiKey);
            _alcoCounterService = alcoCounterService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _botClient.OnMessage += _botClient_OnMessage;
                _botClient.StartReceiving(new UpdateType[] { UpdateType.Message, UpdateType.EditedMessage }, stoppingToken);
                _logger.LogInformation("Telegram bot client started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start bot client");
                throw;
            }
            return Task.CompletedTask;
        }

        private async void _botClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                // todo: научиться обрабатывать отредактированные сообщения
                // todo: сделать multitenancy, что бы для каждого чата вёлся свой учёт, чтобы бота могли добавлять в разные чаты

                _logger.LogTrace("Bot message received: {@message}", e.Message);
                switch (e.Message.Type)
                {
                    case MessageType.ChatMembersAdded:
                        // если самого бота добавили в какой-то чат - новую сущность Chat в базе
                        await _alcoCounterService.CreateOrUpdateChatAsync(e.Message.Chat.Id, e.Message.Chat.Title);
                        break;
                    case MessageType.ChatMemberLeft:
                        // если бота кикнули - наверное не надо ничего делать, из базы не удалять Chat (хотя возможно при передобавлении будет новый ChatId)
                        break;
                    case MessageType.Text:
                        await _alcoCounterService.ProcessMessageAsync(e.Message.Chat.Id, e.Message.From.Id, e.Message.Text);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process bot message");
            }
        }
    }
}
