using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Interaction
{
    [Key]
    [Column(Order = 1)]
    [Required(ErrorMessage = "Укажите пользователя 1")]
    [Display(Name = "Пользователь 1")]
    public int User1Id { get; set; }

    [ForeignKey(nameof(User1Id))]
    public User User1 { get; set; }



    [Key]
    [Column(Order = 2)]
    [Required(ErrorMessage = "Укажите пользователя 2")]
    [Display(Name = "Пользователь 2")]
    public int User2Id { get; set; }

    [ForeignKey(nameof(User2Id))]
    public User User2 { get; set; }



    [Required(ErrorMessage = "Укажите тип отношений")]
    [Display(Name = "Отношения")]
    [EnumDataType(typeof(TypeInteraction))]
    public TypeInteraction Type { get; set; }
}
