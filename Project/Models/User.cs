using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    public int Id { get; set; }



    [Required(ErrorMessage = "Введите почту пользователя")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Почта")]
    public string Email { get; set; }



    [Required(ErrorMessage = "Введите пароль пользователя")]
    [DataType(DataType.Password)]
    [MaxLength(32), MinLength(8)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }



    [Required(ErrorMessage = "Введите имя пользователя")]
    [MaxLength(32), MinLength(3)]
    [Display(Name = "Имя")]
    public string Name { get; set; }



    //public IFormFile? Icon { get; set; }
}
