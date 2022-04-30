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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ModTeamManager.Controllers
{
    [Authorize(Roles = "Moderator,Administrator")]
    public class ChatMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;

        public ChatMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, string channelString, string messageContentString, string serviceString, int? pageNumber)
        {
            ViewData["UserSortParm"] = String.IsNullOrEmpty(sortOrder) ? "user" : "user_asc";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["ServiceSortParm"] = sortOrder == "service" ? "service_desc" : "service";
            ViewData["ChannelSortParm"] = sortOrder == "channel" ? "channel_desc" : "channel";

            var chatmessage = from s in _context.ChatMessage select s;

            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["ChannelFilter"] = channelString;
            ViewData["MessageContentFilter"] = messageContentString;
            ViewData["serviceFilter"] = serviceString;

            if (!String.IsNullOrEmpty(searchString))
            {
                chatmessage = chatmessage.Where(s => s.User.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(channelString))
            {
                chatmessage = chatmessage.Where(s => s.Channel.Contains(channelString));
            }
            if (!String.IsNullOrEmpty(serviceString))
            {
                chatmessage = chatmessage.Where(s => s.Service.Contains(serviceString));
            }
            if (!String.IsNullOrEmpty(messageContentString))
            {
                chatmessage = chatmessage.Where(s => s.Msg.Contains(messageContentString));
            }
            switch (sortOrder)
            {
                case "user":
                    chatmessage = chatmessage.OrderByDescending(s => s.User);
                    break;
                //case "date":
                    //chatmessage = chatmessage.OrderBy(s => s.Date);
                    //break;
                case "date_desc":
                    chatmessage = chatmessage.OrderByDescending(s => s.Date);
                    break;
                case "service":
                    chatmessage = chatmessage.OrderBy(s => s.Service);
                    break;
                case "service_desc": 
                    chatmessage = chatmessage.OrderByDescending(s => s.Service);
                    break;
                case "channel":
                    chatmessage = chatmessage.OrderBy(s => s.Channel) ;
                    break;
                case "channel_desc":
                    chatmessage = chatmessage.OrderByDescending(s => s.Channel);
                    break;
                default:
                    chatmessage = chatmessage.OrderByDescending(s => s.Date);
                    break;
            }
            int pageSize = 100;
            return View(await PaginatedList<ChatMessage>.CreateAsync(chatmessage.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await chatmessage.AsNoTracking().ToListAsync());
        }

        // GET: ChatMessages/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatMessage = await _context.ChatMessage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatMessage == null)
            {
                return NotFound();
            }

            return View(chatMessage);
        }

        // GET: ChatMessages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChatMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Service,Channel,User,Msg,Flag,Url")] ChatMessage chatMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chatMessage);
        }


        // POST: ChatMessages/Ban
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatMessage = await _context.ChatMessage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatMessage == null)
            {
                return NotFound();
            }
            //var httpContext = new HttpContextAccessor().HttpContext;
            //var cur = await _manager.GetUserAsync(httpContext.User);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (chatMessage.UserId == null)
            {
                return NotFound();
            }

            var logEntry = new ModLog
            {
                UserId = chatMessage.UserId,
                Action = "Ban",
                Date = DateTime.UtcNow,
                Login = chatMessage.User,
                Moderator = userEmail
            };
            _context.ModLog.Add(logEntry);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "ModLogs");
        }

        // GET: ChatMessages/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatMessage = await _context.ChatMessage.FindAsync(id);
            if (chatMessage == null)
            {
                return NotFound();
            }
            return View(chatMessage);
        }

        // POST: ChatMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Date,Service,Channel,User,Msg,Flag,Url")] ChatMessage chatMessage)
        {
            if (id != chatMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatMessageExists(chatMessage.Id))
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
            return View(chatMessage);
        }

        // GET: ChatMessages/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatMessage = await _context.ChatMessage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatMessage == null)
            {
                return NotFound();
            }

            return View(chatMessage);
        }

        // POST: ChatMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var chatMessage = await _context.ChatMessage.FindAsync(id);
            _context.ChatMessage.Remove(chatMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatMessageExists(long id)
        {
            return _context.ChatMessage.Any(e => e.Id == id);
        }
    }
}
