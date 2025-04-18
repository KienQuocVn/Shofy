using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shofy.Data;
using Shofy.Models;
using Shofy.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shofy.Pages.Client
{
    public class Pages_Client_ProductModel : PageModel
    {
        private readonly ShofyContext _context; // Thay bằng tên DbContext của bạn
        private readonly ILogger<Pages_Client_ProductModel> _logger;

        public Pages_Client_ProductModel(ShofyContext context, ILogger<Pages_Client_ProductModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Danh sách sản phẩm hiển thị
        public List<Product> Products { get; set; } = new List<Product>();

        // Thông tin sản phẩm cho Modal
        public Product SelectedProduct { get; set; }

        // Tham số tìm kiếm
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        // Tham số lọc giá
        [BindProperty(SupportsGet = true)]
        public string PriceRange { get; set; }

        public async Task OnGetAsync(int? productId)
        {
            // Truy vấn sản phẩm
            IQueryable<Product> productQuery = _context.Product;

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                productQuery = productQuery.Where(p => p.Name.Contains(SearchTerm));
            }

            // Lọc theo khoảng giá
            if (!string.IsNullOrEmpty(PriceRange))
            {
                switch (PriceRange)
                {
                    case "0-50":
                        productQuery = productQuery.Where(p => p.Price >= 0 && p.Price <= 50);
                        break;
                    case "50-100":
                        productQuery = productQuery.Where(p => p.Price > 50 && p.Price <= 100);
                        break;
                    case "100-150":
                        productQuery = productQuery.Where(p => p.Price > 100 && p.Price <= 150);
                        break;
                    case "150-200":
                        productQuery = productQuery.Where(p => p.Price > 150 && p.Price <= 200);
                        break;
                    case "200+":
                        productQuery = productQuery.Where(p => p.Price > 200);
                        break;
                }
            }

            // Lấy danh sách sản phẩm
            Products = await productQuery.ToListAsync();

            // Nếu có productId, lấy thông tin sản phẩm cho Modal
            if (productId.HasValue)
            {
                SelectedProduct = await _context.Product
                    .FirstOrDefaultAsync(p => p.ProductID == productId.Value);
            }
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
            // Logic thêm sản phẩm vào giỏ hàng
            var product = await _context.Product.FindAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return NotFound();
            }

            // Giả sử người dùng đã đăng nhập và có UserID
            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Please log in to add items to cart.";
                // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToPage("/Accounts/Login");
            }

            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (cart == null)
            {
                cart = new Cart { UserID = userId.Value, CreatedAt = DateTime.Now };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductID == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity
                };
                _context.CartItem.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            TempData["CartSuccess"] = $"{product.Name} is added to cart!";
            return RedirectToPage();
        }
    }
}
