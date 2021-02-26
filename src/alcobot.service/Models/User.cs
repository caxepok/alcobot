namespace alcobot.service.Models
{
    /// <summary>
    /// Пользователь, для которого ведётся учёт выпитого
    /// </summary>
    public class User
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
    }
}
