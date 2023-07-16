using DataBase;
using DataBase.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System.Data;

namespace ShopWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        #region Services
        private Context _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IUsersService _applicationUserService;
        private IOrderService _orderService;
        private IShoppingCartService _shoppingCartService;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ILogger<ProductController> _logger;
        public ProductController(Context context
       , IUsersService applicationUserService
       , IOrderService orderService
       , IShoppingCartService shoppingCartService
       , IProductService productService
       , UserManager<IdentityUser> userManager
       , RoleManager<IdentityRole> roleManager
       , SignInManager<IdentityUser> signInManager
       , ILogger<ProductController> logger
       , ICategoryService categoryService
       , IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _categoryService = categoryService;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion
        #region Product section
        [HttpGet]
        [Authorize(Roles = "Superadmin,Administrator")]
        public async Task<IActionResult> UpsertProduct(int? ProductId)
        {
            //دریافت دسته ها و پاس دادن مقادیر به ریزور برای انتخاب دسته محصول جدید

            var listSelected = _categoryService.GetAll().ToList().Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.Name,
            });
            ViewBag.CategoryNames = listSelected;
            if (ProductId == 0 || ProductId == null)
            {
                //چک میکنیم که آیا دسته ای وجود داره داخل دیتا بیس یا نه
                var IsAnyCategoryInDbExist = _categoryService.GetAll().ToList();
                if (IsAnyCategoryInDbExist.Count > 0)
                {
                    var ProductVM = new UpsertProductVM();
                    return View(ProductVM);
                }
                return Redirect("/Admin/Product/UpsertCategory");
            }
            else
            {
                //اگه آیدی نال نبود محصول رو پیدا و برای ویرایش میفرستیم به ریزور
                var product = await _productService.FindFirstOrDefaultObjcetAsync(p => p.id == ProductId);
                if (product != null)
                {
                    var upsertProduct = UpsertProductVM.MergeProducttoProductVM(product);
                    return View(upsertProduct);
                }
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpsertProduct(UpsertProductVM productVM)
        {
            try
            {
                //اخرین محصول رو واکشی میکنیم و برای واضافه کردن عکس میفرستیم به اکشن بعدی
                int newProductId = 0;
                var product = UpsertProductVM.MergeProductVMtoProduct(productVM);
                if (productVM.Id != 0)
                {
                    await _productService.UpdateAsync(product);
                    newProductId = product.id;
                }
                else
                {
                    newProductId = await _productService.AddAsync(product);
                }
                if (newProductId!=-1)
                {
                    return RedirectToAction("SavingProductPhotos", "Product", new { area = "Admin", productId = JsonConvert.SerializeObject(newProductId) });
                }
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Authorize(Roles = "Superadmin,Administrator")]
        public async Task<IActionResult> SavingProductPhotos(string productId)
        {
            var id = JsonConvert.DeserializeObject<int>(productId);
            var product = await _productService.FindFirstOrDefaultObjcetAsync(p => p.id == id);
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> SavingProductPhotos(IFormFile[]? pictures, IFormFile mainPic, string altText, int Id)
        {
            var product = await _productService.FindFirstOrDefaultObjcetAsync(p => p.id == Id);
            //چک میکنیم که آیا محصول مورد نظر عکس داره از قبل تا آپدیتش کنیم و قبلیارو پاک کنیم
            if (product.MainImageAddress != null)
            {
                var rootPath = _hostingEnvironment.WebRootPath;
                var oldPic = Path.Combine(rootPath, product.MainImageAddress.TrimStart('\\'));
                if (System.IO.File.Exists(oldPic))
                {
                    System.IO.File.Delete(oldPic);
                }
            }
            if (product.Images != null)
            {
                foreach (var image in product.Images)
                {
                    var rootPath = _hostingEnvironment.WebRootPath;
                    var oldPic = Path.Combine(rootPath, image.imageAddress.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPic))
                    {
                        System.IO.File.Delete(oldPic);
                    }
                }
            }

            if (mainPic != null)
            {
                var rootPath = _hostingEnvironment.WebRootPath;
                var filename = Guid.NewGuid().ToString();
                var uploadPath = Path.Combine(rootPath, @"images\Products");
                var extension = Path.GetExtension(mainPic.FileName);
                using (var stream = new FileStream(Path.Combine(uploadPath, filename + extension), FileMode.Create))
                {
                    mainPic.CopyTo(stream);
                }
                var imageAddress = "images/Products/" + filename + extension;
                product.MainImageAddress = imageAddress;
            }
            product.Images = new List<ImagesInformation>();
            if (pictures != null)
            {
                foreach (var picture in pictures)
                {
                    var rootPath = _hostingEnvironment.WebRootPath;
                    var filename = Guid.NewGuid().ToString();
                    var uploadPath = Path.Combine(rootPath, @"images\Products");
                    var extension = Path.GetExtension(picture.FileName);
                    using (var stream = new FileStream(Path.Combine(uploadPath, filename + extension), FileMode.Create))
                    {
                        picture.CopyTo(stream);
                    }
                    var imageAddress = "images/Products/" + filename + extension;
                    var imageObj = new ImagesInformation()
                    {
                        imageAddress = imageAddress,
                        productId = product.id
                    };
                    product.Images.Add(imageObj);
                }
            }
            product.picturesAlternativeText = altText;
            var isSaved = _context.SaveChanges();
            if (isSaved > 0)
            {
                return Redirect("/Main/Home/UserAccount");
            }
            return NotFound();
        }
        public IActionResult GetProducts()
        {
            var products = _productService.GetAll().ToList();
            var productsVM = ProductTblVM.tblVM(products);
            return Json(new { data = productsVM });
        }
        [HttpDelete]
        [Authorize(Roles = "Superadmin,Administrator")]
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            var Product = await _productService.FindFirstOrDefaultObjcetAsync(t => t.id == ProductId);
            if (Product != null)
            {
                if (Product.Images != null)
                {
                    foreach (var image in Product.Images)
                    {
                        var rootPath = _hostingEnvironment.WebRootPath;
                        var oldPic = Path.Combine(rootPath, image.imageAddress.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPic))
                        {
                            System.IO.File.Delete(oldPic);
                        }
                    }
                }

                var success = _productService.Remove(Product);
                if (success)
                {
                    return Json(new { success = true, message = "Product" });
                }
                return Json(new { success = false, message = "faild" });
            }
            return Json(new { success = false, message = "faild" });
        }
        #endregion
        #region Category section
        [HttpGet]
        [Authorize(Roles = "Superadmin,Administrator")]
        public async Task<IActionResult> UpsertCategory(int? CategoryId)
        {
            var SelectedCategory = await _categoryService.FindFirstOrDefaultObjcetAsync(c => c.id == CategoryId);
            if (SelectedCategory != null)
                return View(SelectedCategory);
            else
            {
                var NewCategory = new Category();
                return View(NewCategory);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpsertCategory(Category Category)
        {
            try
            {
                if (Category.id != 0)
                    await _categoryService.UpdateAsync(Category);
                else
                    await _categoryService.AddAsync(Category);

                return Redirect("/Main/Home/UserAccount");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Superadmin,Administrator")]
        public async Task<IActionResult> DeleteCategory(int CategoryId)
        {
            var category = await _categoryService.FindFirstOrDefaultObjcetAsync(t => t.id == CategoryId);
            if (category != null)
            {
                var success = _categoryService.Remove(category);
                if (success)
                {
                    return Json(new { success = true, message = "Category" });
                }
                return Json(new { success = false, message = "faild" });
            }
            return Json(new { success = false, message = "faild" });
        }
        public IActionResult GetCategories()
        {
            var categories = _categoryService.GetAll().ToList();
            return Json(new { data = categories });
        }
        #endregion
    }
}
