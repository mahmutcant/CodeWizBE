using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeWizBE.Entities
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
        public string UserMessage { get; set; }
        public string MessageContent { get; set; }
        public Chat Chat { get; set; }
    }
}
