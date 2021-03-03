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
                new VolumeRegex() { RegExText = "����", Milliliters = 500, },
                new VolumeRegex() { RegExText = "����", Milliliters = 568, },
                new VolumeRegex() { RegExText = "�����", Milliliters = 750, },
                new VolumeRegex() { RegExText = "����", Milliliters = 18, },
                new VolumeRegex() { RegExText = "�����", Milliliters = 200, },
                new VolumeRegex() { RegExText = "�����", Milliliters = 200, },
                new VolumeRegex() { RegExText = "����", Milliliters = 1000, },
                // new VolumeRegex() { RegExText = "�", Milliliters = 1000, },
                new VolumeRegex() { RegExText = "�������", Milliliters = 500, },
                new VolumeRegex() { RegExText = "����", Milliliters = 500 },
                new VolumeRegex() { RegExText = "�����", Milliliters = 330 },
                new VolumeRegex() { RegExText = "��", Milliliters = 1 } },
                new Alcohole[] {
                    new Alcohole() { RegExText = new string[] { "����", "����" }, DrinkType = Enums.DrinkType.Beer },
                    new Alcohole() { RegExText = new string[] { "�����", "�����" }, DrinkType = Enums.DrinkType.Vodka },
                    new Alcohole() { RegExText = new string[] { "���", "����" }, DrinkType = Enums.DrinkType.Rum },
                    new Alcohole() { RegExText = new string[] { "����������", "�����������" }, DrinkType = Enums.DrinkType.Champagne },
                    new Alcohole() { RegExText = new string[] { "����", "����" }, DrinkType = Enums.DrinkType.Moonshine }
                }
                );
        }

        [Fact]
        public void RegexTests()
        {
            var drinks = _messageParserService.ParseMessageToDrinks("0.25 ����, 1 ���� �����, 500 �����, ���� ����� 200 �����, 0.25 ����, ������� �����������");
            Assert.Equal(2, drinks.Count());
        }
    }
}