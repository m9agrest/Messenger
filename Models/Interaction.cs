using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SimpleMessenger.Models
{
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

    public enum TypeInteraction
    {
        [Display(Name = "Обоюдный бан")]
        Enemy = -3,
        [Display(Name = "Пользователь 1 заблокировал 2")]
        Blocked = -2,
        [Display(Name = "Пользователь 2 заблокировал 1")]
        Blocker = -1,
        [Display(Name = "Нейтральные")]
        None = 0,
        [Display(Name = "Пользователь 1 подписан на 2")]
        Subscriber = 1,
        [Display(Name = "Пользователь 2 подписан на 1")]
        Subscription = 2,
        [Display(Name = "Друзья")]
        Friend = 3,
    }
}
