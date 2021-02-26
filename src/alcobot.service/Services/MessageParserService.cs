using alcobot.service.Models;
using alcobot.service.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Services
{
    /// <inheritdoc cref="IMessageParserService"/>
    public class MessageParserService : IMessageParserService
    {
        private readonly ILogger _logger;

        public MessageParserService(ILogger<MessageParserService> logger)
        {
            _logger = logger;
        }

        public Drink[] ParseMessageToDrinks(string message)
        {
            // todo: наваять парсер дринков
            return new Drink[] { new Drink() { Volume = 200, DrinkType = Enums.DrinkType.Beer } };
            // return Enumerable.Empty<Drink>();
        }
    }
}
