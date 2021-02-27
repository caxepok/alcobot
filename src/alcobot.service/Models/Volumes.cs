using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Models
{
    /// <summary>
    /// Каталог, по которому парсятся объёмы выпитого
    /// </summary>
    public class VolumeRegex
    {
        public long Id { get; set; }
        public string RegExText { get; set; }
        public int Milliliters { get; set; }
    }
}
