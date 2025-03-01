using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacilityManagement.Models;

namespace FacilityManagement.Controllers
{
    public class UsagesController : Controller
    {
        private readonly FacilityManagementContext _context;

        public UsagesController(FacilityManagementContext context)
        {
            _context = context;
        }

        // GET: Usages
        public async Task<IActionResult> Index()
        {
            var facilityManagementContext = _context.Usages.Include(u => u.Room);
            return View(await facilityManagementContext.ToListAsync());
        }

        // GET: Usages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usage = await _context.Usages
                .Include(u => u.Room)
                .FirstOrDefaultAsync(m => m.UsageId == id);
            if (usage == null)
            {
                return NotFound();
            }

            return View(usage);
        }

        // GET: Usages/Create
        public IActionResult Create()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            // Check if CustomerId exists in session
            if (customerId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để sử dụng chức năng này.";
                return RedirectToAction("Index", "LoginC");
            }
            var staffName = HttpContext.Session.GetString("CustomerLogin");

            // Tạo một đối tượng Usage mới và gán giá trị cho UsedBy
            var usage = new Usage
            {
                UsedBy = staffName // Tự động điền tên nhân viên vào UsedBy
            };

            // Tạo danh sách các phòng và gán vào ViewBag
            ViewBag.RoomId = new SelectList(_context.Rooms, "RoomId", "RoomName");

            return View(usage);
        }

        // POST: Usages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsageId,RoomId,UsedBy,UsageDate,Purpose")] Usage usage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", usage.RoomId);
            return View(usage);
        }

        // GET: Usages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usage = await _context.Usages.FindAsync(id);
            if (usage == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", usage.RoomId);
            return View(usage);
        }

        // POST: Usages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsageId,RoomId,UsedBy,UsageDate,Purpose")] Usage usage)
        {
            if (id != usage.UsageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsageExists(usage.UsageId))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", usage.RoomId);
            return View(usage);
        }

        // GET: Usages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usage = await _context.Usages
                .Include(u => u.Room)
                .FirstOrDefaultAsync(m => m.UsageId == id);
            if (usage == null)
            {
                return NotFound();
            }

            return View(usage);
        }

        // POST: Usages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usage = await _context.Usages.FindAsync(id);
            if (usage != null)
            {
                _context.Usages.Remove(usage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsageExists(int id)
        {
            return _context.Usages.Any(e => e.UsageId == id);
        }
    }
}
