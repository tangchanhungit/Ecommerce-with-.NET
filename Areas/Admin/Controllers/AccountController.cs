using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_web_app.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AccountController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Login()
		{
			return View();
		}
	}
}
