using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

public class Message
{
    [Key]
    public int Id { get; set; }



    [Column(Order = 1)]
    [ForeignKey(nameof(Sender))]
    public int SenderId { get; set; }

    [Required(ErrorMessage = "Укажите отправителя")]
    [Display(Name = "Отправитель")]
    public User Sender { get; set; }



    [Column(Order = 2)]
    [ForeignKey(nameof(Recipient))]
    public int RecipientId { get; set; }

    [Required(ErrorMessage = "Укажите получателя")]
    [Display(Name = "Получатель")]
    public User Recipient { get; set; }



    [DataType(DataType.DateTime)]
    [Display(Name = "Время отправления")]
    public DateTime Date { get; set; } = DateTime.Now;



    [DataType(DataType.DateTime)]
    [Display(Name = "Время изменения")]
    public DateTime DateEdit { get; set; } = DateTime.Now;



    //public IFormFile? Photo { get; set; }



    [MaxLength(1000)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Текст сообщения")]
    public string Text { get; set; } = "";



    //public bool Check { get; set; } = false;
}
