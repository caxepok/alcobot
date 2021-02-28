using alcobot.service.Models;
using System.Collections.Generic;

namespace alcobot.service.Services.Interfaces
{
    /// <summary>
    /// Парсер сообщений в дринки
    /// </summary>
    public interface IMessageParserService
    {
        /// <summary>
        /// Парсит сообщение и возвращает коллекцию дринков, которые передал пользователь
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <returns>дринки</returns>
        IEnumerable<Drink> ParseMessageToDrinks(string message);
        /// <summary>
        /// Инициализация парсера
        /// </summary>
        /// <param name="volumeRegexes">регулярки объёмов</param>
        /// <param name="alcoholes">регулярки типов алкоголя</param>
        void Initialize(IEnumerable<VolumeRegex> volumeRegexes, IEnumerable<Alcohole> alcoholes);
        string DescribeDrink(Drink drink);
    }
}
