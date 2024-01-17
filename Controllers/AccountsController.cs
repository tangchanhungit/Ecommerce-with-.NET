using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Ecommerce_web_app.Areas.Admin.Models;
using Ecommerce_web_app.Extentions;
using Ecommerce_web_app.Helpper;
using Ecommerce_web_app.Models;
using Ecommerce_web_app.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChangePasswordViewModel = Ecommerce_web_app.ModelViews.ChangePasswordViewModel;

namespace Ecommerce_web_app.Controllers
{
	[Authorize]
	public class AccountsController : Controller
	{
		private readonly EcommerceContext _context;
		public INotyfService _notifyService { get; }
		public AccountsController(EcommerceContext context, INotyfService notifyService)
		{
			_context = context;
			_notifyService = notifyService;
		}
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ValidatePhone(string phone)
		{
			try
			{
				var khachhang = _context.Customers.SingleOrDefault(x => x.Phone.ToLower() == phone.ToLower());
				if (khachhang != null)
				{
					return Json(data: " Số điện thoại: " + phone + "Đã được sử dụng");
				}
				return Json(data: true);
			}
			catch
			{
				return Json(data: true);
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ValidateEmail(string Email)
		{
			try
			{
				var khachhang = _context.Customers.SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
				if (khachhang != null)
				{
					return Json(data: " Email: " + Email + "Đã được sử dụng");
				}
				return Json(data: true);
			}
			catch
			{
				return Json(data: true);
			}
		}
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
		{
			var taikhoanId = HttpContext.Session.GetString("CustomerId");
			if(taikhoanId != null)
			{
				var khachhang = _context.Customers.AsNoTracking()
					.SingleOrDefault(x=>x.CustomerId==Convert.ToInt32(taikhoanId));

				var lsOrder = _context.Orders.AsNoTracking()
					.Include(x=>x.TranscatStatus)
					.Where(x=>x.CustomerId == khachhang.CustomerId)
					.OrderByDescending(x=>x.OrderDate)
					.ToList();
				ViewBag.DonHang = lsOrder;

				if (khachhang != null)
				{
					return View(khachhang);
				}
			}
			return RedirectToAction("Login");
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("dang-ky.html", Name = "DangKy")]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("dang-ky.html", Name = "DangKy")]
		public async Task<IActionResult> Register(RegisterViewModel user)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string salt = Utilities.GetRandomKey();
					Customer khachhang = new Customer
					{
						FullName = user.FullName,
						Phone = user.Phone.Trim().ToLower(),
						Email = user.Email.Trim().ToLower(),
						Password = (user.Password + salt.Trim()).ToMD5(),
						Active = true,
						Salt = salt,
						CreateDate = DateTime.Now

					};
					try
					{
						_context.Add(khachhang);
						await _context.SaveChangesAsync();
						HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
						var taikhoanId = HttpContext.Session.GetString("CustomerId");
						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, khachhang.FullName),
							new Claim("CustomerId", khachhang.CustomerId.ToString())
						};
						ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
						ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
						await HttpContext.SignInAsync(claimsPrincipal);
						return RedirectToAction("Dashboard", "Accounts");

					}
					catch(Exception e)
					{
						return RedirectToAction("Register", "Accounts");
					}
				}
				else
				{
					return View(user);
				}
			}
			catch
			{
				return View(user);
			}
		}

		[AllowAnonymous]
		[Route("dang-nhap.html", Name = "DangNhap")]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			var taikhoanId = HttpContext.Session.GetString("CustomerId");
			if (taikhoanId != null)
			{
				return RedirectToAction("Dashboard", "Accounts");
			}
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("dang-nhap.html", Name = "DangNhap")]
		public async Task<IActionResult> Login(LoginModelView customer)
		{
			try
			{
				if (ModelState.IsValid)
				{
					bool isEmail = Utilities.IsValidEmail(customer.UserName);
					if (!isEmail)
					{
						return View(customer);
					}

                    var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim().Equals(customer.UserName.Trim()));

					if (khachhang == null)
					{

						return RedirectToAction("Register");
					}

					string pass = (customer.Password + khachhang.Salt.Trim() ).ToMD5();
					
					if (khachhang.Password != pass)
					{
						
						_notifyService.Success("Thông tin đăng nhập chưa chính xác.");
						return View(customer);
					}
					if (khachhang.Active == false)
					{
						return RedirectToAction("ThongBao", "Accounts");
					}

					HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
					var taikhoanId = HttpContext.Session.GetString("CustomerId");
					var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, khachhang.FullName),
							new Claim("CustomerId", khachhang.CustomerId.ToString())
						};
					ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
					await HttpContext.SignInAsync(claimsPrincipal);
					_notifyService.Success("Đăng nhập thành công");
					return RedirectToAction("Dashboard", "Accounts");
				}
			}
			catch
			{
				return RedirectToAction("Register", "Accounts");
			}
			return View(customer);
		}
        [HttpGet]
		[Route("dang-xuat.html", Name ="Logout")]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync();
			HttpContext.Session.Remove("CustomerId");
			return RedirectToAction("Index", "Home");
		}

		public IActionResult ChangePassword(ChangePasswordViewModel model)
		{
			try
			{
				var taikhoanId = HttpContext.Session.GetString("CustomerId");
				if (taikhoanId == null)
				{
					return RedirectToAction("Login", "Accounts");
				}
				if (ModelState.IsValid)
				{
					var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanId));
					if (taikhoan == null)
					{
						return RedirectToAction("Login", "Accounts");
					}
					var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
					if (pass == taikhoan.Password)
					{
						string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
						taikhoan.Password = passnew;
						_context.Update(taikhoan);
						_context.SaveChanges();
						_notifyService.Success("Đổi mật khẩu thành công");
						return RedirectToAction("Dashboard", "Accounts");
					}
				}
			}
			catch
			{
				_notifyService.Error("Đổi mật khẩu thất bại");
				return RedirectToAction("Dashboard", "Accounts");

			}
			_notifyService.Error("Đổi mật khẩu thất bại");
			return RedirectToAction("Dashboard", "Accounts");
		}
	}
}


