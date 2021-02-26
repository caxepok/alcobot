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

        private void _botClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                // todo: научиться обрабатывать отредактированные сообщения
                // todo: сделать multitenancy, что бы для каждого чата вёлся свой учёт, чтобы бота могли добавлять в разные чаты
                _alcoCounterService.ProcessMessage(e.Message.Chat.Id, e.Message.From.Id, e.Message.Text);
                _logger.LogTrace("Bot message received: {@message}", e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process bot message");
            }
        }
    }
}
