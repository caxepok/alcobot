using alcobot.service.Models;
using System.IO;
using System.Threading.Tasks;

namespace alcobot.service.Services.Interfaces
{
    /// <summary>
    /// Сервис учёта алкоголя
    /// </summary>
    public interface IAlcoCounterService
    {
        /// <summary>
        /// Обработка сообщения о выпитом
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="username">имя пользователя</param>
        /// <param name="message">текст сообщения</param>
        /// <returns></returns>
        Task<string> ProcessMessageAsync(long chatId, long userId, string username, string message);
        /// <summary>
        /// Создание нового чата
        /// </summary>
        /// <param name="id">идентификатор чата</param>
        /// <param name="title">название чата</param>
        Task CreateOrUpdateChatAsync(long id, string title);
        /// <summary>
        /// Экспорт записанного ботом в csv
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <param name="userId">идентификатор пользователя</param>
        /// <returns>csv файл</returns>
        Task<byte[]> ExportAsync(long chatId, long userId);
        /// <summary>
        /// Удаляет последнюю запись, внесённую пользователем
        /// </summary>
        /// <param name="chatId">идентификатор чата</param>
        /// <param name="userId">идентификатор пользователя</param>
        /// <returns>удалённая запись</returns>
        Task<Drink> DeleteLastRecordAsync(long chatId, long userId);
        string DesribeDrink(Drink drink);
    }
}
