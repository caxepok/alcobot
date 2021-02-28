namespace alcobot.service.Models
{
    /// <summary>
    /// Пользователь, для которого ведётся учёт выпитого
    /// </summary>
    public class Drinker
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
    }
}
