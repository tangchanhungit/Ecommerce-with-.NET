using Ecommerce_web_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_web_app.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceContext _context;
        public ProductController(EcommerceContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x=>x.ProductId==id);
            if(product==null) {
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}
