using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace work_2.Controllers
{
    public class InteractionsController : Controller
    {
        private readonly MessengerDBContext _context;

        public InteractionsController(MessengerDBContext context)
        {
            _context = context;
        }

        // GET: Interactions
        public async Task<IActionResult> Index()
        {
            var messengerDBContext = _context.Interaction.Include(i => i.User1).Include(i => i.User2);
            return View(await messengerDBContext.ToListAsync());
        }

        // GET: Interactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Interaction == null)
            {
                return NotFound();
            }

            var interaction = await _context.Interaction
                .Include(i => i.User1)
                .Include(i => i.User2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (interaction == null)
            {
                return NotFound();
            }

            return View(interaction);
        }

        // GET: Interactions/Create
        public IActionResult Create()
        {
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Email");
            ViewData["User2Id"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: Interactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,User1Id,User2Id,Type")] Interaction interaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(interaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Email", interaction.User1Id);
            ViewData["User2Id"] = new SelectList(_context.User, "Id", "Email", interaction.User2Id);
            return View(interaction);
        }

        // GET: Interactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Interaction == null)
            {
                return NotFound();
            }

            var interaction = await _context.Interaction.FindAsync(id);
            if (interaction == null)
            {
                return NotFound();
            }
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Email", interaction.User1Id);
            ViewData["User2Id"] = new SelectList(_context.User, "Id", "Email", interaction.User2Id);
            return View(interaction);
        }

        // POST: Interactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,User1Id,User2Id,Type")] Interaction interaction)
        {
            if (id != interaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(interaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InteractionExists(interaction.Id))
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
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Email", interaction.User1Id);
            ViewData["User2Id"] = new SelectList(_context.User, "Id", "Email", interaction.User2Id);
            return View(interaction);
        }

        // GET: Interactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Interaction == null)
            {
                return NotFound();
            }

            var interaction = await _context.Interaction
                .Include(i => i.User1)
                .Include(i => i.User2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (interaction == null)
            {
                return NotFound();
            }

            return View(interaction);
        }

        // POST: Interactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Interaction == null)
            {
                return Problem("Entity set 'MessengerDBContext.Interaction'  is null.");
            }
            var interaction = await _context.Interaction.FindAsync(id);
            if (interaction != null)
            {
                _context.Interaction.Remove(interaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InteractionExists(int id)
        {
          return (_context.Interaction?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
