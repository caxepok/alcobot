using alcobot.service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Models
{
    /// <summary>
    /// Тип алкоголя
    /// </summary>
    public class Alcohole
    {
        public long Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Тип напитка
        /// </summary>
        public DrinkType DrinkType {get;set;}
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
