using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SimpleMessenger.Data;
using SimpleMessenger.Models;
using Interaction = SimpleMessenger.Models.Interaction;

namespace SimpleMessenger.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        private User getUser()
        {
            if(_httpContextAccessor.HttpContext.Session.GetInt32("UserId") != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == _httpContextAccessor.HttpContext.Session.GetInt32("UserId"));
                if(user != null){
                    return user;
                }
                _httpContextAccessor.HttpContext.Session.Remove("UserId");
            }
            return null;
        }

        [HttpGet]
        public IActionResult Index(int id) 
        {
            if (id <= 0)
            {
                var user = getUser();
                if(user != null)
                { 
                    return View("profile", user);
                }
                return RedirectToAction("Login");
            }
            else
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    var me = getUser();
                    if(user.Id == me.Id)
                    {
						return View("profile", user);
					}
                    Account account = new Account()
                    {
                        Session = me != null,
                        User = user,
                        Interaction = me != null ?
                            _context.Interactions.FirstOrDefault(
                                i =>
                                    (i.User1Id == user.Id && i.User2Id == me.Id) ||
                                    (i.User2Id == user.Id && i.User1Id == me.Id)
                            ) 
                            : null
                    };
                    return View("user", account);
                }
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (existingUser != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("UserId", existingUser.Id);
                return RedirectToAction("");
            }
            ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");
            return View("login", user);
        }

        [HttpPost]
        public IActionResult Registration(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser == null)
            {
                existingUser = _context.Users.FirstOrDefault(u => u.Name == user.Name);
                if (existingUser == null)
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    var U = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    _httpContextAccessor.HttpContext.Session.SetInt32("UserId", U.Id);
                    return RedirectToAction("");
                }

                ModelState.AddModelError(string.Empty, "имя уже используется");
                return View("registration", user);
            }
            ModelState.AddModelError(string.Empty, "логин уже используется");
            return View("registration", user);
        }

        public IActionResult Login()
        {
            if(getUser() != null)
            {
                return RedirectToAction("");
            }
            return View("login");
        }
        public IActionResult Registration()
        {
            if (getUser() != null)
            {
                return RedirectToAction("");
            }
            return View("registration");
        }

        [HttpPost]
        public IActionResult Interaction(int id, int type)
		{
			var me = getUser();
            if(me != null && me.Id != id)
			{
				var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if(user != null)
                {
                    var interaction = _context.Interactions.FirstOrDefault(
                        i => 
                            (i.User1Id == me.Id && i.User2Id == user.Id) || 
                            (i.User2Id == me.Id && i.User1Id == user.Id));
                    var inter = interaction != null ? interaction.Type : TypeInteraction.None;
                    if (
                        (inter == TypeInteraction.None && type == 0) ||
                        (inter == TypeInteraction.Friend && type == 1) ||
                        (inter == TypeInteraction.Enemy && type == -1))
                    { }
                    else
					{
						Console.WriteLine(type);
						Console.WriteLine(type);
						Console.WriteLine(type);
						Console.WriteLine(type);
						Console.WriteLine(type);
						Console.WriteLine(type);
						if (me.Id < user.Id)
						{
                            if (inter == TypeInteraction.None && type == 1) inter = TypeInteraction.Subscriber;
							else if (inter == TypeInteraction.Subscription && type == 1) inter = TypeInteraction.Friend;
                            else if (inter == TypeInteraction.Subscriber && type == 0) inter = TypeInteraction.None;
                            else if (inter == TypeInteraction.Blocked && type == 0) inter = TypeInteraction.None;
                            else if (inter == TypeInteraction.Enemy && type == 0) inter = TypeInteraction.Blocker;
                            else if (inter == TypeInteraction.Blocker && type == -1) inter = TypeInteraction.Enemy;
							else if (type == -1) inter = TypeInteraction.Blocked;
						}
						else
						{
							if (inter == TypeInteraction.None && type == 1) inter = TypeInteraction.Subscription;
							else if (inter == TypeInteraction.Subscriber && type == 1) inter = TypeInteraction.Friend;
							else if (inter == TypeInteraction.Subscription && type == 0) inter = TypeInteraction.None;
							else if (inter == TypeInteraction.Blocker && type == 0) inter = TypeInteraction.None;
							else if (inter == TypeInteraction.Enemy && type == 0) inter = TypeInteraction.Blocked;
							else if (inter == TypeInteraction.Blocked && type == -1) inter = TypeInteraction.Enemy;
							else if (type == -1) inter = TypeInteraction.Blocker;
						}
						Console.WriteLine(type);
						Console.WriteLine(type);
						Console.WriteLine(type);
						Console.WriteLine(type);
						Console.WriteLine(type);
						if (interaction != null)
                        {
							interaction.Type = inter;
							_context.Interactions.Update(interaction);
							_context.SaveChanges();
						}
                        else
						{
							interaction = new Interaction
                            {
                                User1Id = me.Id < user.Id ? me.Id : user.Id,
								User2Id = me.Id > user.Id ? me.Id : user.Id,
								Type = inter
							};

							_context.Interactions.Add(interaction);
							_context.SaveChanges();
						}
					}
				}
			}
            return RedirectToAction("", new { id = id });
		}

        [HttpPost]
        public IActionResult Update(string name, IFormFile photo)
        {
            var user = getUser();
            if(user == null)
            {
                return NotFound();
            }
            var check = _context.Users.FirstOrDefault(user=> user.Name == name);
            if(check == null && name != null )
            {
                user.Name = name;
            }
            if(photo != null && photo.Length > 0)
            {
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                user.Photo = Path.GetFileName(filePath);
            }
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("", new { id = user.Id });
        }

        public class Account
        {
            public User User;
            public Interaction Interaction;
            public bool Session;
        }
    }
}