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
                new VolumeRegex() { RegExText = "����", Milliliters = 1000, },
                new VolumeRegex() { RegExText = "������", Milliliters = 500, } },
                new Alcohole[] {
                    new Alcohole() { RegExText = new string[] { "����", "����" } },
                    new Alcohole() { RegExText = new string[] { "�����","�����" } } }
                );
        }

        [Fact]
        public void RegexTests()
        {
            var drinks = _messageParserService.ParseMessageToDrinks("0.33 ���� 200 �����");
            Assert.Equal(2, drinks.Count());
        }
    }
}