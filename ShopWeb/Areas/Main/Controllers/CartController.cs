using DataBase;
using DataBase.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace ShopWeb.Areas.Main.Controllers
{
    [Area("Main")]
    [Authorize]
    public class CartController : Controller
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
        public CartController(Context context
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
        public IActionResult Cart()
        {
            //محصولات سفارش داده شده توسط کاربر رو واکشی میکنیم
            var id = _userManager.GetUserId(User);
            var allUserCarts = _shoppingCartService.FindAllUserCarts(id).ToList();
            //قیمت کل آخر قایل پرداخت
            var cartvm = new CartVM();
            foreach (var cart in allUserCarts)
            {
                #region Price Calculation
                cart.totalPrice = cart.Count * cart.Product.Price;
                cartvm.lastPrice += (decimal)cart.totalPrice;
                #endregion
            }
            cartvm.carts = allUserCarts;

            return View(cartvm);
        }
        [HttpPost]
        public async Task<IActionResult> Cart(CartVM updateValues)
        {
            var cartvm = new CartVM();
            var userId = _userManager.GetUserId(User);
            var allUserCarts = _shoppingCartService.FindAllUserCarts(userId).ToList();

            if (updateValues != null && updateValues.carts.Count > 0)
            {
                foreach (var value in updateValues.carts)
                {
                    var product = allUserCarts.FirstOrDefault(c => c.Id == value.Id);
                    if (product != null)
                    {
                        var isSaved = _shoppingCartService.increamentCount(product, value.Count);
                    }
                }
            }
            cartvm.carts = allUserCarts;
            foreach (var cart in cartvm.carts)
            {
                // Price Calculation
                cart.totalPrice = cart.Count * cart.Product.Price;
                cartvm.lastPrice += (decimal)cart.totalPrice;
            }

            return View(cartvm);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            var cart = await _shoppingCartService.FindFirstOrDefaultObjcetAsync(c => c.Id == cartId);
            var deleted = _shoppingCartService.Remove(cart);

            // Return appropriate response
            if (deleted)
            {
                return Ok(new { success = true });
            }
            else
            {
                return BadRequest(new { success = false });
            }
        }

    }
}
