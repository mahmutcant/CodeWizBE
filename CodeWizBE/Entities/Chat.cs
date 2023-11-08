using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeWizBE.Entities
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public User User { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}