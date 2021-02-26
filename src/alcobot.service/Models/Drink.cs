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
        public long UserId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        // todo: может добавить место, где выпил?
        /// <summary>
        /// Объём выпитого в мл
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// Тип выпитого
        /// </summary>
        public DrinkType DrinkType { get; set; }
    }
}
