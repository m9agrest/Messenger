using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleMessenger.Data;
using SimpleMessenger.Models;

namespace SimpleMessenger.Controllers
{
    public class MessagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessagesController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
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

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return NotFound();
            }
            var appDbContext = _context.Messages.Include(m => m.Recipient).Include(m => m.Sender);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = getUser();
            if (user == null || user.Id >= Program.Admin)
            {
                return NotFound();
            }
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

    }
}
