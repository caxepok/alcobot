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
        Drink[] ParseMessageToDrinks(string message);
    }
}
