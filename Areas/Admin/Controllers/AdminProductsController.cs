using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce_web_app.Models;
using PagedList.Core;
using Ecommerce_web_app.Helpper;

namespace Ecommerce_web_app.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AdminProductsController : Controller
	{
		private readonly EcommerceContext _context;

		public AdminProductsController(EcommerceContext context)
		{
			_context = context;
		}

		// GET: Admin/AdminProducts
		public async Task<IActionResult> Index(int? page, int CatID = 0)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 20;

			List<Product> lsProducts = new List<Product>();

			if (CatID != 0)
			{
				lsProducts = _context.Products
					.AsNoTracking().Include(x => x.Cat)
					.Where(x => x.CatId == CatID)
					.Include(x => x.Cat)
					.OrderByDescending(x => x.ProductId).ToList();
			}
			else
			{
				lsProducts = _context.Products.AsNoTracking().Include(x => x.Cat).OrderByDescending(x => x.ProductId).ToList();
			}

			PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
			ViewBag.CurrentCatID = CatID;
			ViewBag.CurrentPage = pageNumber;
			ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);

			return View(models);
		}

		public IActionResult Filter(int CatID = 0)
		{
			var url = $"/Admin/AdminProducts?CatID={CatID}";
			if (CatID == 0)
			{
				url = $"/Admin/AdminProducts";
			}
			return Json(new { status = "success", redirectUrl = url });
		}

		// GET: Admin/AdminProducts/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Cat)
				.FirstOrDefaultAsync(m => m.ProductId == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// GET: Admin/AdminProducts/Create
		public IActionResult Create()
		{
			ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
			return View();
		}

		// POST: Admin/AdminProducts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Desciption,Price,Discount,CatId,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslStock")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
		{
			if (ModelState.IsValid)
			{

				product.ProductName = Utilities.ToTitleCase(product.ProductName);
				if (fThumb != null)
				{
					string extension = Path.GetExtension(fThumb.FileName);
					string image = Utilities.SEOUrl(product.ProductName) + extension;
					product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
				}
				if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
				product.Alias = Utilities.SEOUrl(product.ProductName);
				product.DateCreated = DateTime.Now;
				product.DateModified = DateTime.Now;
				_context.Add(product);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
			return View(product);
		}

		// GET: Admin/AdminProducts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
			return View(product);
		}

		// POST: Admin/AdminProducts/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Desciption,Price,Discount,CatId,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslStock")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
		{
			if (id != product.ProductId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					product.ProductName = Utilities.ToTitleCase(product.ProductName);
					if (fThumb != null)
					{
						string extension = Path.GetExtension(fThumb.FileName);
						string image = Utilities.SEOUrl(product.ProductName) + extension;
						product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
					}
					if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
					product.Alias = Utilities.SEOUrl(product.ProductName);

					product.DateModified = DateTime.Now;
					_context.Update(product);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductExists(product.ProductId))
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
			ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
			return View(product);
		}

		// GET: Admin/AdminProducts/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Cat)
				.FirstOrDefaultAsync(m => m.ProductId == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST: Admin/AdminProducts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Products == null)
			{
				return Problem("Entity set 'EcommerceContext.Products'  is null.");
			}
			var product = await _context.Products.FindAsync(id);
			if (product != null)
			{
				_context.Products.Remove(product);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProductExists(int id)
		{
			return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
		}
	}
}
