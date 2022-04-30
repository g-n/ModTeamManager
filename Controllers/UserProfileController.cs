using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModTeamManager.Data;
using ModTeamManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace ModTeamManager.Controllers
{
    [Authorize(Roles = "Moderator,Administrator")]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TwitchUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserProfile.ToListAsync());
        }

        // GET: TwitchUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var twitchUser = await _context.UserProfile
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (twitchUser == null)
            {
                return NotFound();
            }
            return View(twitchUser);
        }

        // GET: TwitchUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TwitchUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Login,CreatedOn")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userProfile);
        }

        // GET: TwitchUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var twitchUser = await _context.UserProfile.FindAsync(id);
            if (twitchUser == null)
            {
                return NotFound();
            }
            return View(twitchUser);
        }

        // POST: TwitchUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Login,CreatedOn")] UserProfile userProfile)
        {
            if (id != userProfile.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TwitchUserExists(userProfile.UserId))
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
            return View(userProfile);
        }

        // GET: TwitchUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var twitchUser = await _context.UserProfile
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (twitchUser == null)
            {
                return NotFound();
            }

            return View(twitchUser);
        }

        // POST: TwitchUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var twitchUser = await _context.UserProfile.FindAsync(id);
            _context.UserProfile.Remove(twitchUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TwitchUserExists(int id)
        {
            return _context.UserProfile.Any(e => e.UserId == id);
        }
    }
}
