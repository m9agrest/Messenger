using System.ComponentModel.DataAnnotations;

namespace SimpleMessenger.Models
{
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

        [Display(Name = "Фотография")]
        public string? Photo { get; set; } // URL фотографии пользователя
    }
}
