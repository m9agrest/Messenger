using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace work_2.Controllers
{
    public class MessagesController : Controller
    {
        private readonly MessengerDBContext _context;

        public MessagesController(MessengerDBContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var messengerDBContext = _context.Chat.Include(m => m.Recipient).Include(m => m.Sender);
            return View(await messengerDBContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Chat == null)
            {
                return NotFound();
            }

            var message = await _context.Chat
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["RecipientId"] = new SelectList(_context.User, "Id", "Email");
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SenderId,RecipientId,Date,DateEdit,Text")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipientId"] = new SelectList(_context.User, "Id", "Email", message.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "Email", message.SenderId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Chat == null)
            {
                return NotFound();
            }

            var message = await _context.Chat.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["RecipientId"] = new SelectList(_context.User, "Id", "Email", message.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "Email", message.SenderId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SenderId,RecipientId,Date,DateEdit,Text")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipientId"] = new SelectList(_context.User, "Id", "Email", message.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.User, "Id", "Email", message.SenderId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Chat == null)
            {
                return NotFound();
            }

            var message = await _context.Chat
                .Include(m => m.Recipient)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Chat == null)
            {
                return Problem("Entity set 'MessengerDBContext.Chat'  is null.");
            }
            var message = await _context.Chat.FindAsync(id);
            if (message != null)
            {
                _context.Chat.Remove(message);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
          return (_context.Chat?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
