using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SimpleMessenger.Data;
using SimpleMessenger.Models;

namespace SimpleMessenger.Controllers
{
	public class ChatController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ChatController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Index(int id)
        {
            var me = getUser();
            if(me == null || id == 0 || me.Id == id)
            {
                return NotFound();
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            var interaction = _context.Interactions.FirstOrDefault(i => i.User1Id == me.Id && i.User2Id == user.Id ||
                                                                        i.User2Id == me.Id && i.User1Id == user.Id);
            ChatData data = new ChatData();
            data.Me = me;
            data.User = user;
            data.Blocked = interaction == null ? false : (int)interaction.Type < 0;
            var messages = _context.Messages.Where(m => m.SenderId == me.Id && m.RecipientId == user.Id || m.SenderId == user.Id && m.RecipientId == me.Id).ToList();
            data.Messages = messages;
            return View("chat", data);
        }
        [HttpPost]
        public IActionResult Send(int id, string text, IFormFile photo)
        {
            var me = getUser();
            if (me == null || id == 0 || me.Id == id)
            {
                return NotFound();
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var interaction = _context.Interactions.FirstOrDefault(i => i.User1Id == me.Id && i.User2Id == user.Id ||
                                                                        i.User2Id == me.Id && i.User1Id == user.Id);
            if(interaction != null && (int)interaction.Type < 0)
            {
                return RedirectToAction("", new { id = id });
            }
            if(text == null && (photo == null || photo.Length == 0))
            {
                return RedirectToAction("", new { id = id });
            }

            var msg = new Message
            {
                SenderId = me.Id,
                RecipientId = user.Id,
                Text = text,
                Date = DateTime.Now,
                DateEdit = DateTime.Now
            };

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

                msg.Photo = Path.GetFileName(filePath);
            }

            _context.Messages.Add(msg);
            _context.SaveChanges();

            return RedirectToAction("", new { id = id });
        }

        public class ChatData
        {
            public User Me;
            public User User;
            public List<Message> Messages;
            public bool Blocked;
        }



    }
}
