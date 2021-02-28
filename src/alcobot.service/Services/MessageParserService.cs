using alcobot.service.Enums;
using alcobot.service.Models;
using alcobot.service.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace alcobot.service.Services
{
    /// <inheritdoc cref="IMessageParserService"/>
    public class MessageParserService : IMessageParserService
    {
        private readonly ILogger _logger;
        private Regex _regexVolumes;
        private Regex _regexAlcoholes;
        private IEnumerable<VolumeRegex> _volumes;
        private IEnumerable<Alcohole> _alcoholes;

        public MessageParserService(ILogger<MessageParserService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Drink> ParseMessageToDrinks(string message)
        {
            // 1. RegEx parser
            var volumes = _regexVolumes.Matches(message);
            var alcoholes = _regexAlcoholes.Matches(message);
            // nothing found
            if (!volumes.Any() || !alcoholes.Any())
                return Enumerable.Empty<Drink>();

            List<Drink> drinks = new List<Drink>();
            for (int i = 0; i < volumes.Count; i++)
            {
                drinks.Add(new Drink()
                {
                    Volume = GetVolume(volumes[i].Value),
                    DrinkType = GetType(alcoholes[i].Value)
                });
            }
            return drinks;

            // Если ничего не нашли, то можно как-то по-другому ещё
        }

        public string DescribeDrink(Drink drink) =>
            $"{_alcoholes.Single(_ => _.DrinkType == drink.DrinkType).Name} {drink.Volume} мл";

        private int GetVolume(string value)
        {
            if(Decimal.TryParse(value.Replace(',','.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var volume))
            {
                // determine volume in milliliters from digit
                if (volume <= 0)
                    throw new InvalidOperationException("Volume must me greater than 0");
                if (volume == 1)
                    return 1000;
                if (volume < 1)
                    return (int)(volume * 1000);
                return (int)volume;
            }
            else
            {
                // determine volume from volumes catalogue
                return _volumes.Single(_ => _.RegExText == value).Milliliters;
            }
        }

        private DrinkType GetType(string value) =>
            _alcoholes.Single(_ => _.RegExText.Contains(value)).DrinkType;

        public void Initialize(IEnumerable<VolumeRegex> dbVolumes, IEnumerable<Alcohole> dbAlcoholes)
        {
            _volumes = dbVolumes;
            _alcoholes = dbAlcoholes;
            // (?<volume>\d+[\.,]?\d+|\d+|литр|стопка|стопарь|кружка|стакан|бокал|банка)
            // (?<alcohol>пиво|пива|водка|водки|водочки)

            // дробные объёмы, 0.250, 0,33, 0,5 в литрах
            // точные объёмы, 330, 500, 1000 в миллилитрах
            string volumesFromDatabase = String.Join('|', dbVolumes.Select(_ => _.RegExText.ToLowerInvariant()));
            string alcoholesFromDatabase = String.Join('|', dbAlcoholes.Select(_ => String.Join('|', _.RegExText.Select(s => s.ToLowerInvariant()))));
            string volumesRegEx = @$"(?<volume>\d+[\.,]?\d+|\d+|{volumesFromDatabase})";
            string alcoholesRegEx = @$"(?<alcohol>{alcoholesFromDatabase})";

            _regexVolumes = new Regex(volumesRegEx, RegexOptions.Compiled);
            _regexAlcoholes = new Regex(alcoholesRegEx, RegexOptions.Compiled);

            _logger.LogInformation("Message parser initialized");
            _logger.LogInformation($"Volumes: {volumesRegEx}");
            _logger.LogInformation($"Alcoholes: {alcoholesRegEx}");
        }
    }
}
