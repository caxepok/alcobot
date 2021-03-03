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
                new VolumeRegex() { RegExText = "круж", Milliliters = 500, },
                new VolumeRegex() { RegExText = "пинт", Milliliters = 568, },
                new VolumeRegex() { RegExText = "бутыл", Milliliters = 750, },
                new VolumeRegex() { RegExText = "глот", Milliliters = 18, },
                new VolumeRegex() { RegExText = "бокал", Milliliters = 200, },
                new VolumeRegex() { RegExText = "фужер", Milliliters = 200, },
                new VolumeRegex() { RegExText = "литр", Milliliters = 1000, },
                // new VolumeRegex() { RegExText = "л", Milliliters = 1000, },
                new VolumeRegex() { RegExText = "поллитр", Milliliters = 500, },
                new VolumeRegex() { RegExText = "банк", Milliliters = 500 },
                new VolumeRegex() { RegExText = "баноч", Milliliters = 330 },
                new VolumeRegex() { RegExText = "мл", Milliliters = 1 } },
                new Alcohole[] {
                    new Alcohole() { RegExText = new string[] { "пиво", "пива" }, DrinkType = Enums.DrinkType.Beer },
                    new Alcohole() { RegExText = new string[] { "водка", "водки" }, DrinkType = Enums.DrinkType.Vodka },
                    new Alcohole() { RegExText = new string[] { "ром", "рома" }, DrinkType = Enums.DrinkType.Rum },
                    new Alcohole() { RegExText = new string[] { "шампанское", "шампанского" }, DrinkType = Enums.DrinkType.Champagne },
                    new Alcohole() { RegExText = new string[] { "чача", "чачи" }, DrinkType = Enums.DrinkType.Moonshine },
                    new Alcohole() { RegExText = new string[] { "бехеровка", "бехеровки" }, DrinkType = Enums.DrinkType.Liquor }
                }
                );
        }

        [Fact]
        public void RegexTests()
        {
            var drinks = _messageParserService.ParseMessageToDrinks("0.25 пива, 1 литр водки, 500 водки, литр водки 200 водка, 0.25 чачи, бутылка шампанского, 100 бехеровки");
            Assert.Equal(2, drinks.Count());
        }
    }
}