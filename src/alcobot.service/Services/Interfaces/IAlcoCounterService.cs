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
        Task ProcessMessageAsync(long chatId, long userId, string username, string message);
        /// <summary>
        /// Создание нового чата
        /// </summary>
        /// <param name="id">идентификатор чата</param>
        /// <param name="title">название чата</param>
        Task CreateOrUpdateChatAsync(long id, string title);
    }
}
