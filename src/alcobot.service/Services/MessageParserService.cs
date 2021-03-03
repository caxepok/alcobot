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
        private Regex _regexFull;
        private IEnumerable<VolumeRegex> _volumes;
        private IEnumerable<Alcohole> _alcoholes;

        public MessageParserService(ILogger<MessageParserService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Drink> ParseMessageToDrinks(string message)
        {
            // 1. RegEx parser
            var full = _regexFull.Matches(message);
            // nothing found
            if (!full.Any())
                return Enumerable.Empty<Drink>();

            List<Drink> drinks = new List<Drink>();
            foreach (Match match in full)
            {
                string volume = match.Groups["volume"].Value;
                string measure = match.Groups["measure"].Value;
                string alcohol = match.Groups["alcohol"].Value;
                int ml = GetVolumeInMilliliters(volume, measure);
                DrinkType type = GetDrinkType(alcohol);
                drinks.Add(new Drink() { DrinkType = type, Volume = ml });
            }
            return drinks;

            // Если ничего не нашли, то можно как-то по-другому ещё
        }

        public string DescribeDrink(Drink drink) =>
            $"{_alcoholes.Single(_ => _.DrinkType == drink.DrinkType).Name} {drink.Volume} мл";

        private int GetVolumeInMilliliters(string volumeAsString, string measure)
        {
            // 1 литр пива
            // 0.25 пива
            // литр пива

            decimal volume = 1;
            if (volumeAsString != String.Empty)
            {
                volume = Decimal.Parse(volumeAsString.Replace(',', '.'), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture);
            }

            if (measure == String.Empty)
            {
                if (volume < 1)
                    return (int)(volume * 1000);
                return (int)volume;
            }

            if (measure == "л") // хак для литров, пока и так сойдет :)
                return (int)(volume * 1000);

            return (int)(_volumes.Single(_ => measure.StartsWith(_.RegExText)).Milliliters * volume);
        }

        private DrinkType GetDrinkType(string value) =>
            _alcoholes.Single(_ => _.RegExText.Contains(value)).DrinkType;

        public void Initialize(IEnumerable<VolumeRegex> dbVolumes, IEnumerable<Alcohole> dbAlcoholes)
        {
            _volumes = dbVolumes;
            _alcoholes = dbAlcoholes;
            // дробные объёмы, 0.250, 0,33, 0,5 в литрах
            // точные объёмы, 330, 500, 1000 в миллилитрах
            string volumesFromDatabase = String.Join('|', dbVolumes.Select(_ => _.RegExText.ToLowerInvariant()));
            string alcoholesFromDatabase = String.Join('|', dbAlcoholes.Select(_ => String.Join('|', _.RegExText.Select(s => s.ToLowerInvariant()))));
            string fullRegEx = $@"(?<volume>\d*[.,]*\d+)*\s*(?<measure>({volumesFromDatabase})+\w*)*[\s,]*(?<alcohol>({alcoholesFromDatabase})+[a-z]*)";

            _regexFull = new Regex(fullRegEx, RegexOptions.Compiled);

            _logger.LogInformation("Message parser initialized");
            _logger.LogInformation($"Volumes: {volumesFromDatabase}");
            _logger.LogInformation($"Alcoholes: {alcoholesFromDatabase}");
            _logger.LogInformation($"RegExp: {fullRegEx}");
        }
    }
}
