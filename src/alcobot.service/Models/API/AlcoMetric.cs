using alcobot.service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace alcobot.service.Models.API
{
    public class AlcoMetric
    {
        public long UserId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public int TotalVolume { get; set; }
        public int LightVolume { get; set; }
        public int MediumVolume { get; set; }
        public int StrongVolume { get; set; }
        public int UltraVolume { get; set; }
        public IEnumerable<Drink> Drinks { get; set; }

        public AlcoMetric(List<Drink> drinks)
        {
            Drinks = drinks;
            TotalVolume = drinks.Sum(_ => _.Volume);
            LightVolume = drinks.Where(_ => GetStrenght(_.DrinkType) == DrinkStrength.Light).Sum(_ => _.Volume);
            MediumVolume = drinks.Where(_ => GetStrenght(_.DrinkType) == DrinkStrength.Medium).Sum(_ => _.Volume);
            StrongVolume = drinks.Where(_ => GetStrenght(_.DrinkType) == DrinkStrength.Strong).Sum(_ => _.Volume);
            UltraVolume = drinks.Where(_ => GetStrenght(_.DrinkType) == DrinkStrength.Ultra).Sum(_ => _.Volume);
        }

        public static DrinkStrength GetStrenght(DrinkType drinkType) => drinkType switch
        {
            DrinkType.Absinthe => DrinkStrength.Ultra,

            DrinkType.Beer or
            DrinkType.Champagne or
            DrinkType.Cider or
            DrinkType.Wine => DrinkStrength.Light,

            DrinkType.Brandy or
            DrinkType.Cognac or
            DrinkType.Gin or
            DrinkType.Liquor or
            DrinkType.Moonshine or
            DrinkType.Rum or
            DrinkType.Sambuka or
            DrinkType.Tequila or
            DrinkType.Vodka or
            DrinkType.Spirit or
            DrinkType.Nalewka or
            DrinkType.Wiskey => DrinkStrength.Strong,

            DrinkType.StrongBeer or 
            DrinkType.Portwine or
            DrinkType.Vermouth => DrinkStrength.Medium,

            _ => throw new ArgumentOutOfRangeException(nameof(DrinkType)),
        };

        internal string Describe()
        {
            StringBuilder sb = new StringBuilder();
            if (LightVolume > 0)
                sb.AppendLine($"Лёгкого алкоголя: {LightVolume} мл");
            if (MediumVolume > 0)
                sb.AppendLine($"Среднего алкоголя: {MediumVolume} мл");
            if (StrongVolume > 0)
                sb.AppendLine($"Крепкого алкоголя: {StrongVolume} мл");
            if (UltraVolume > 0)
                sb.AppendLine($"Сверхкрепкого: {UltraVolume} мл");

            return sb.ToString();
        }
    }
}
