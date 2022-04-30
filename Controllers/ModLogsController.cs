#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModTeamManager.Data;
using ModTeamManager.Models;

namespace ModTeamManager.Views
{
    public class ModLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ModLogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.ModLog.ToListAsync());
        }

        // GET: ModLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modLog = await _context.ModLog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modLog == null)
            {
                return NotFound();
            }

            return View(modLog);
        }

        // GET: ModLogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ModLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Moderator,UserId,Login,Action,Date")] ModLog modLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modLog);
        }

        // GET: ModLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modLog = await _context.ModLog.FindAsync(id);
            if (modLog == null)
            {
                return NotFound();
            }
            return View(modLog);
        }

        // POST: ModLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Moderator,UserId,Login,Action,Date")] ModLog modLog)
        {
            if (id != modLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModLogExists(modLog.Id))
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
            return View(modLog);
        }

        // GET: ModLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modLog = await _context.ModLog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modLog == null)
            {
                return NotFound();
            }

            return View(modLog);
        }

        // POST: ModLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modLog = await _context.ModLog.FindAsync(id);
            _context.ModLog.Remove(modLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModLogExists(int id)
        {
            return _context.ModLog.Any(e => e.Id == id);
        }
    }
}
