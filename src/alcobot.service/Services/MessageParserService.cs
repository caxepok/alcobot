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
            var full = _regexFull.Matches(message);
            if (!full.Any())
                return Enumerable.Empty<Drink>();

            List<Drink> drinks = new List<Drink>();
            foreach (Match match in full)
            {
                string alcohol = match.Groups["alcohol"].Value;
                if (alcohol == String.Empty)
                    throw new InvalidOperationException("Невозможно определить тип алкогольного напитка");
                string volume = match.Groups["volume"].Value;
                string measure = match.Groups["measure"].Value;

                int ml = GetVolumeInMilliliters(volume, measure);
                Alcohole alcohole = _alcoholes.Single(_ => _.RegExText.Contains(alcohol));
                drinks.Add(new Drink() { AlcoholId = alcohole.Id, DrinkType = alcohole.DrinkType, Volume = ml });
            }
            return drinks;
            // todo: может если ничего не нашли, то можно как-то по-другому ещё попробовать поискать
        }

        public string DescribeDrink(Drink drink)
        {
            var alcohol = _alcoholes.Single(_ => _.Id == drink.AlcoholId);
            string name = alcohol.Emoji == null ? alcohol.Name : $"{alcohol.Name} {alcohol.Emoji}";
            return $"{name} {drink.Volume} мл";
        }

        /// <summary>
        /// Возвращает объём выпитого. Если единица измерения не указана то считается что это в миллилитрах
        /// </summary>
        /// <param name="volumeAsString">объём</param>
        /// <param name="measureAsString">единица измерения</param>
        /// <returns>объём в миллилитрах</returns>
        private int GetVolumeInMilliliters(string volumeAsString, string measureAsString)
        {
            if (volumeAsString == String.Empty && measureAsString == String.Empty)
                throw new InvalidOperationException("Невозможно определить объём алкогольного напитка");

            decimal volume = 1;
            if (volumeAsString != String.Empty)
            {
                try
                {
                    volume = Decimal.Parse(volumeAsString.Replace(',', '.'), System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to parse volume as decimal: {volumeAsString}");
                    throw new InvalidOperationException("Невозможно определить объём алкогольного напитка");
                }
            }

            if (measureAsString == String.Empty)
            {
                if (volume < 1)
                    return (int)(volume * 1000);
                return (int)volume;
            }

            if (measureAsString == "л") // хак для литров, пока и так сойдет :)
                return (int)(volume * 1000);

            return (int)(_volumes.Single(_ => measureAsString.StartsWith(_.RegExText)).Milliliters * volume);
        }

        /// <summary>
        /// Инициализация сервиса
        /// </summary>
        /// <param name="dbVolumes">каталог объёмов</param>
        /// <param name="dbAlcoholes">каталог алкогольных напитков</param>
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
