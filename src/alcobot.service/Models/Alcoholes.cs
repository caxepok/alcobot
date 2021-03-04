using alcobot.service.Enums;

namespace alcobot.service.Models
{
    /// <summary>
    /// Тип алкоголя
    /// </summary>
    public class Alcohole
    {
        public long Id { get; set; }
        /// <summary>
        /// Название алкогольного напитка
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Эмоджи
        /// </summary>
        public string Emoji { get; set; }
        /// <summary>
        /// Тип напитка
        /// </summary>
        public DrinkType DrinkType { get; set; }
        /// <summary>
        /// Средняя крепость алкоголя
        /// </summary>
        public decimal AverageStrength { get; set; }
        /// <summary>
        /// Строка для парсинга типа алкоголя
        /// </summary>
        public string[] RegExText { get; set; }
    }
}
