using Ecommerce_web_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_web_app.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class SearchController : Controller
	{
		private readonly EcommerceContext _context;
		public SearchController(EcommerceContext context)
		{
			_context = context;
		}
		[HttpPost]
		public IActionResult FindProduct(string keyword)
		{
			List<Product> products = new List<Product>();
			if(string.IsNullOrEmpty(keyword)|| keyword.Length < 1) 
			{
				return PartialView("ListProductSearchPartial", null);
			}
			products = _context.Products
				.AsNoTracking()
				.Include(a=>a.Cat)
				.Where(x=>x.ProductName.Contains(keyword))
				.OrderByDescending(x=>x.ProductName)
				.Take(10)
				.ToList();
			if(products == null)
			{
				return PartialView("ListProductSearchPartial", null);
			}
			else
			{
				return PartialView("ListProductSearchPartial", products);
			}
		}
		public IActionResult Index()
		{
			return View();
		}
	}
}
