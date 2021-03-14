namespace alcobot.service.Infrastructure
{
    public class Declension
    {
        /// <summary>
        /// Does a word declension after a number.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nominative">nominative (минута)</param>
        /// <param name="genitiveSingular">genitiveSingular (минуты)</param>
        /// <param name="genitivePlural">genitivePlural (минут)</param>
        /// <returns></returns>
        public static string GetDeclensionForNumber(int number, string nominative, string genitiveSingular, string genitivePlural)
        {
            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;
            if (lastDigit == 1 && lastTwoDigits != 11)
            {
                return nominative;
            }
            if (lastDigit == 2 && lastTwoDigits != 12 || lastDigit == 3 && lastTwoDigits != 13 || lastDigit == 4 && lastTwoDigits != 14)
            {
                return genitiveSingular;
            }
            return genitivePlural;
        }
    }
}
