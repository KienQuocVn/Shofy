using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Shofy.Models;
using Shofy.Data; // namespace chứa ShofyContext
using System.ComponentModel.DataAnnotations;

namespace Shofy.Pages.Admin
{
    public class CreateProductModel(ShofyContext context, IWebHostEnvironment environment) : PageModel
    {
        private readonly ShofyContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

        [BindProperty, Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [BindProperty, Required, StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [BindProperty, Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [BindProperty, Required, Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [BindProperty, Required]
        public IFormFile UploadImage { get; set; } = default!;

        [BindProperty, Required]
        public string Status { get; set; } = "Active";

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            string imageFileName = "default.jpg";
            if (UploadImage != null)
            {
                var fileName = Path.GetFileName(UploadImage.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    UploadImage.CopyTo(stream);
                }

                imageFileName = fileName;
            }

            var product = new Product
            {
                Name = Name,
                Description = Description,
                Price = Price,
                StockQuantity = StockQuantity,
                ImagePath = imageFileName,
                Status = Status,
                CreatedDate = DateTime.Now
            };

            _context.Product.Add(product);
            _context.SaveChanges();

            return RedirectToPage("/Admin/Products");
        }
    }
}
