using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Interaction
{
    [Key]
    public int Id { get; set; }



    [Column(Order = 1)]
    [ForeignKey(nameof(User1))]
    public int User1Id { get; set; }

    [Required(ErrorMessage = "Укажите пользователя 1")]
    [Display(Name = "Пользователь 1")]
    public User User1 { get; set; }



    [Column(Order = 2)]
    [ForeignKey(nameof(User2))]
    public int User2Id { get; set; }

    [Required(ErrorMessage = "Укажите пользователя 2")]
    [Display(Name = "Пользователь 2")]
    public User User2 { get; set; }



    [Required(ErrorMessage = "Укажите тип отношений")]
    [Display(Name = "Отношения")]
    [EnumDataType(typeof(TypeInteraction))]
    public TypeInteraction Type { get; set; }
}
