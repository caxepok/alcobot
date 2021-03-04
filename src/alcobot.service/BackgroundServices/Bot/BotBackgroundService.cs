﻿using alcobot.service.Infrastructure;
using alcobot.service.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace alcobot.service.BackgroundServices.Bot
{
    public class BotBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _botClient;
        private readonly IAlcoCounterService _alcoCounterService;
        private string _botUsername;

        public BotBackgroundService(ILogger<BotBackgroundService> logger, IOptions<AppSettings> options, IAlcoCounterService alcoCounterService)
        {
            _logger = logger;
            _botClient = new TelegramBotClient(options.Value.BotApiKey);
            _alcoCounterService = alcoCounterService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var me = await _botClient.GetMeAsync(stoppingToken);
                _botUsername = $"@{me.Username}";
                _botClient.OnMessage += _botClient_OnMessage;
                _botClient.StartReceiving(new UpdateType[] { UpdateType.Message, UpdateType.EditedMessage }, stoppingToken);
                _logger.LogInformation("Telegram bot client started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start bot client");
                throw;
            }
        }

        private async void _botClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                // todo: научиться обрабатывать отредактированные сообщения
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
                        string response = null;
                        if (e.Message.Chat.Type == ChatType.Private)
                        {
                            if (await IsBotCommand(e.Message))
                                return;
                            response = await _alcoCounterService.ProcessMessageAsync(e.Message.Chat.Id, e.Message.From.Id, e.Message.From.Username, e.Message.Text);
                        }
                        else
                        {
                            // detect if bot mentioned
                            if (!IsMentioned(e.Message))
                                return;
                            response = await _alcoCounterService.ProcessMessageAsync(e.Message.Chat.Id, e.Message.From.Id, e.Message.From.Username, e.Message.Text);
                        }
                        if (response != null)
                            await _botClient.SendTextMessageAsync(e.Message.Chat.Id, response);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process bot message");
            }
        }

        /// <summary>
        /// Проверяет сообщение на наличие в нём команды, обрабатывает если нужно
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <returns>true - сообщение команда, false - не команда</returns>
        private async Task<bool> IsBotCommand(Message message)
        {
            if (IsBotCommand(message, "/start"))
            {
                await ProcessBotStartCommand(message.Chat.Id);
                return true;
            }
            if (IsBotCommand(message, "/help"))
            {
                await ProcessBotHelpCommand(message.Chat.Id);
                return true;
            }
            if (IsBotCommand(message, "/last"))
            {
                await ProcessBotLastCommand(message.Chat.Id, message.From.Id);
                return true;
            }
            if (IsBotCommand(message, "/export"))
            {
                await ProcessBotExportCommand(message.Chat.Id, message.From.Id);
                return true;
            }
            return false;
        }

        private Task ProcessBotStartCommand(long chatId) =>
            _botClient.SendTextMessageAsync(chatId, 
                "Привет, я алкобот, помогу тебе записывать количество выпитого алкоголя. Просто присылай мне сообщения вида: \"0,5 пива\" в личку или с упоминанием в любом чате где я есть и я всё запомню ;)");

        private Task ProcessBotHelpCommand(long chatId) =>
            _botClient.SendTextMessageAsync(chatId,
                "Пиши сначала количество алкоголя, потом тип алкоголя, например \"литр шампанского\", \"50 виски\", \"банка пива\", \"5 стаканов рома\"");
        
        private async Task ProcessBotExportCommand(long chatId, long userId)
        {
            byte[] data = await _alcoCounterService.ExportAsync(chatId, userId);
            InputOnlineFile file = new InputOnlineFile(new MemoryStream(data), $"Alcodunk bot export {DateTimeOffset.Now:dd.MM.yyyy HH:mm}");
            await _botClient.SendDocumentAsync(chatId, file);
        }

        private async Task ProcessBotLastCommand(long chatId, long userId)
        {
            // todo: 
            throw new NotImplementedException();
        }

        /// <summary>
        /// Возвращает true если бот упомянут в сообщении
        /// </summary>
        /// <param name="message">сообщение</param>
        private bool IsMentioned(Message message) =>
            message.Entities != null &&
            message.Entities.Any(_ => _.Type == MessageEntityType.Mention) &&
            message.EntityValues.Any(_ => _ == _botUsername);

        /// <summary>
        /// Возвращает true если это команда для бота
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="command">команда (включая /)</param>
        private bool IsBotCommand(Message message, string command) =>
            message.Entities != null &&
            message.Entities.Any(_ => _.Type == MessageEntityType.BotCommand) &&
            message.EntityValues.Any(_ => _ == command);
    }
}
