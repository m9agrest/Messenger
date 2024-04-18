using System.ComponentModel.DataAnnotations;

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
