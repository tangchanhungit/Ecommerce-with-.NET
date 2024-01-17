using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce_web_app.Models;
using Microsoft.IdentityModel.Tokens;
using Ecommerce_web_app.Helpper;
using Ecommerce_web_app.Extentions;
using Ecommerce_web_app.Areas.Admin.Models;

namespace Ecommerce_web_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly EcommerceContext _context;

        public AdminUsersController(EcommerceContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminUsers
        public async Task<IActionResult> Index()
        {

			ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");

			List<SelectListItem> lsActive = new List<SelectListItem>();
			lsActive.Add(new SelectListItem() { Text = "Active", Value = "1" });
			lsActive.Add(new SelectListItem() { Text = "Block", Value = "0" });
			ViewData["ListActive"] = lsActive;

			var ecommerceContext = _context.Users.Include(u => u.Role);

            return View(await ecommerceContext.ToListAsync());
        }

        // GET: Admin/AdminUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/AdminUsers/Create
        public IActionResult Create()
        {
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Admin/AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Email,Password,Active,FullName,RoleId,LastLogin,CreateDate,Salt")] User user)
        {
            if (ModelState.IsValid)
            {

                string salt = Utilities.GetRandomKey();
                user.Salt = salt;

                user.Password = (user.FullName.ToLower() + salt.Trim()).ToMD5();
                user.CreateDate = DateTime.Now;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
			ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

		// GET: Admin/AdminUsers/ChangePassword
		public IActionResult ChangePassword()
		{
			ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName");
			return View();
		}
        // GET: Admin/AdminUsers/ChangePassword
        [HttpPost]
		public IActionResult ChangePassword(ChangePasswordViewModel model)
		{
            if (ModelState.IsValid)
            {
                var taikhoan = _context.Users.AsNoTracking().SingleOrDefault(x => x.Email == model.Email);
                if (taikhoan == null) return RedirectToAction("Login", "Users");
                var pass = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                if(pass == taikhoan.Password)
                {
                    string passNew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    taikhoan.Password = passNew;
                    taikhoan.LastLogin = DateTime.Now;
                    _context.Update(taikhoan);
                    _context.SaveChanges();
                    RedirectToAction("Login", "Users", new {Area = "Admin"});
                }
            }
			return View();
		}


		// GET: Admin/AdminUsers/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Admin/AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Email,Password,Active,FullName,RoleId,LastLogin,CreateDate,Salt")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }

        // GET: Admin/AdminUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'EcommerceContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
