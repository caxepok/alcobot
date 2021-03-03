using alcobot.service.Enums;
using System;

namespace alcobot.service.Models
{
    /// <summary>
    /// Запись выпитого
    /// </summary>
    public class Drink
    {
        public long Id { get; set; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// Время когда пользователь написал сообщение
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
        /// <summary>
        /// Идентификатор чата, где пользователь написал это
        /// </summary>
        public long ChatId { get; set; }
        /// <summary>
        /// Объём выпитого в мл
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// Тип выпитого
        /// </summary>
        public DrinkType DrinkType { get; set; }
        /// <summary>
        /// Идентификатор из каталога алкоголя
        /// </summary>
        public long AlcoholId { get; set; }

        // todo: может добавить место, где выпил?
    }
}
