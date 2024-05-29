using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleMessenger.Data;
using SimpleMessenger.Models;

namespace SimpleMessenger.Controllers
{
    public class ListController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ListController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        private User getUser()
        {
            if (_httpContextAccessor.HttpContext.Session.GetInt32("UserId") != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == _httpContextAccessor.HttpContext.Session.GetInt32("UserId"));
                if (user != null)
                {
                    return user;
                }
                _httpContextAccessor.HttpContext.Session.Remove("UserId");
            }
            return null;
        }
        private List<User> getUsers(User user, TypeInteraction T1, TypeInteraction T2)
        {
            List<Interaction> interactions = _context.Interactions.Where(
                i => 
                    (i.User1Id == user.Id && i.Type == T1) ||
                    (i.User2Id == user.Id && i.Type == T2)
                ).ToList();
            List<User> users = new List<User>();
            foreach (Interaction interaction in interactions)
            {
                int id = interaction.User1Id == user.Id ? interaction.User2Id : interaction.User1Id;
                var U = _context.Users.FirstOrDefault(u => u.Id == id);
                if (U != null)
                {
                    users.Add(U);
                }
            }
            return users;
        }


        public IActionResult Index()
        {
            var user = getUser();
            if (user != null)
            {
                return View("list", _context.Users.ToList());
            }
            return NotFound();
        }
        public IActionResult Friend()
        {
            var user = getUser();
            if (user != null)
            {
                return View("list", getUsers(user, TypeInteraction.Friend, TypeInteraction.Friend));
            }
            return NotFound();
        }
        public IActionResult Subscriber()
        {
            var user = getUser();
            if (user != null)
            {
                return View("list", getUsers(user, TypeInteraction.Subscription, TypeInteraction.Subscriber));
            }
            return NotFound();
        }
        public IActionResult Subscription()
        {
            var user = getUser();
            if (user != null)
            {
                return View("list", getUsers(user, TypeInteraction.Subscriber, TypeInteraction.Subscription));
            }
            return NotFound();
        }
        public IActionResult Blocked()
        {
            var user = getUser();
            if (user != null)
            {
                return View("list", getUsers(user, TypeInteraction.Blocked, TypeInteraction.Blocker).Concat(getUsers(user, TypeInteraction.Enemy, TypeInteraction.Enemy)));
            }
            return NotFound();
        }
    }
}
