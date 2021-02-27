using alcobot.service.Models;
using alcobot.service.Services;
using alcobot.service.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Xunit;

namespace alcobot.service.tests
{
    public class AlcodrunkServiceTests : IClassFixture<OptionsFixture>
    {
        private readonly OptionsFixture _options;
        private readonly IMessageParserService _messageParserService;

        public AlcodrunkServiceTests(OptionsFixture options)
        {
            ILoggerFactory _loggerFactory = options.ServiceProvider.GetRequiredService<ILoggerFactory>();
            _messageParserService = new MessageParserService(_loggerFactory.CreateLogger<MessageParserService>());

            _messageParserService.Initialize(new VolumeRegex[] {
                new VolumeRegex() { RegExText = "литр", Milliliters = 1000, },
                new VolumeRegex() { RegExText = "кружка", Milliliters = 500, } },
                new Alcohole[] {
                    new Alcohole() { RegExText = new string[] { "пиво", "пива" } },
                    new Alcohole() { RegExText = new string[] { "водка","водки" } } }
                );
        }

        [Fact]
        public void RegexTests()
        {
            var drinks = _messageParserService.ParseMessageToDrinks("0.33 пиво 200 водка");
            Assert.Equal(2, drinks.Count());
        }
    }
}