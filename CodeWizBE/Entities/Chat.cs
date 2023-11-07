namespace CodeWizBE.Entities
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public User User { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}