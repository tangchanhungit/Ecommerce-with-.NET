using AspNetCoreHero.ToastNotification.Abstractions;
using Ecommerce_web_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_web_app.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly EcommerceContext _context;
        public INotyfService _notifyService { get; }
        public ShoppingCartController(EcommerceContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        /*public List<CartItem> GioHang
        {
            get
            {
                var gh = Htt]
            }
        }*/
        public IActionResult Index()
        {
            return View();
        }
    }
}
