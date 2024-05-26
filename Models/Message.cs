using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SimpleMessenger.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Column(Order = 1)]
        [Required(ErrorMessage = "Укажите отправителя")]
        [Display(Name = "Отправитель")]
        public int SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; }

        [Column(Order = 2)]
        [Required(ErrorMessage = "Укажите получателя")]
        [Display(Name = "Получатель")]
        public int RecipientId { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public User Recipient { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Время отправления")]
        public DateTime Date { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Время изменения")]
        public DateTime DateEdit { get; set; }

        [MaxLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Текст сообщения")]
        public string? Text { get; set; }

        [Display(Name = "Фотография")]
        public string? Photo { get; set; } // URL фотографии сообщения
    }
}
