using DataBase;
using DataBase.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using System.Security.Claims;

namespace ShopWeb.Areas.Main.Controllers
{
    [Area("Main")]
    public class UserProductController : Controller
    {
        #region Services
        private Context _context;
        private IUsersService _applicationUserService;
        private IOrderService _orderService;
        private IShoppingCartService _shoppingCartService;
        private IProductService _productService;
        private RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ILogger<HomeController> _logger;
        public UserProductController(Context context
       , IUsersService applicationUserService
       , IOrderService orderService
       , IShoppingCartService shoppingCartService
       , IProductService productService
       , UserManager<IdentityUser> userManager
       , RoleManager<IdentityRole> roleManager
       , SignInManager<IdentityUser> signInManager
       , ILogger<HomeController> logger)
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
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> ProductDetail(int productId)
        {
            var product = await _productService.FindFirstOrDefaultObjcetAsync(p => p.id == productId);
            var additionalPictures = new List<ImagesInformation>();
            if (product.Images.Count > 4)
            {
                additionalPictures = product.Images.Take(4).ToList();
            }
            else
            {
                additionalPictures = product.Images;
            }
            var productsWithSameCategory = _productService.GetAll(p => p.CategoryId == product.CategoryId).ToList();
            //عدد شش برای این است که میخاهیم فقط شش تا از محصولات مرطبت و کمتر در ویو نشان داده شود نه بیشتر
            if (productsWithSameCategory.Count > 6)
            {
                productsWithSameCategory.RemoveRange(6, productsWithSameCategory.Count - 6);
            }
            if (product != null)
            {
                var productDetailVM = new ProductDetailVM
                {
                    product = product,
                    relatedProducts = productsWithSameCategory,
                    productCommnets = _productService.FetchProductsComments(productId),
                };
                if (User.Identity.IsAuthenticated)
                {
                    var id = _userManager.GetUserId(User);
                    var cart = await _shoppingCartService.FindFirstOrDefaultObjcetAsync(c=>c.ProductId==productDetailVM.product.id
                    &&c.UserId==id);
                    if (cart!=null)
                    {
                        productDetailVM.countCart = cart.Count;
                    }
                }
                return View(productDetailVM);
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProductDetail(int cartCount, string size, int productVMId)
        {
            var product = await _productService.FindFirstOrDefaultObjcetAsync(p => p.id == productVMId);
            if (product != null)
            {
                if (size != null)
                    product.Size = size;
                else
                    product.Size = "Medium";

                var id = _userManager.GetUserId(User);
                var allUserProductsInCart= _shoppingCartService.FindAllUserCarts(id);
                var shoppingCartItemExist = allUserProductsInCart.FirstOrDefault(c=>c.ProductId == productVMId);
                bool isSuccess = false;
                int shoppingCartCount = 0;
                if (shoppingCartItemExist == null)
                {
                    var shoppingCart = new ShoppingCartItem()
                    {
                        Count = cartCount,
                        ProductId = product.id,
                        UserId = id
                    };
                    isSuccess = await _shoppingCartService.AddAsync(shoppingCart);
                }
                else
                {
                    shoppingCartCount = _shoppingCartService.increamentCount(shoppingCartItemExist, cartCount);
                }
                if (shoppingCartCount != -1)
                {
                    return Redirect("/Main/Home/Index");
                }
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComments(ProductDetailVM detailvm, int productId)
        {
            var comment = detailvm.comments;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment != null && productId != 0)
            {
                var settedComment = _applicationUserService.SetComments(comment, productId, userId);
                if (settedComment)
                {
                    TempData["SubmitComment"] = "نظر شما با موفقیت ثبت شد";
                    return RedirectToAction("ProductDetail", "UserProduct", new { area = "Main", productId = productId });
                }
            }
            return NotFound();
        }
    }
}
