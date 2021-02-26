﻿using alcobot.service.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace alcobot.service.Services
{
    public class AlcoCounterService : IAlcoCounterService
    {
        private readonly ILogger _logger;

        public AlcoCounterService(ILogger<AlcoCounterService> logger)
        {
            _logger = logger;
        }

        public Task ProcessMessage(long chatId, long userId, string message)
        {
            // todo: распарсить алкоголь, положить в базу
            return Task.CompletedTask;
        }
    }
}
