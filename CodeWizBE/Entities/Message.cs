namespace CodeWizBE.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        public Chat Chat { get; set; }
    }
}
