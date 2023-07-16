using DataBase;
using DataBase.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ShopWeb.Areas.Main.Controllers
{
    [Area("Main")]
    public class HomeController : Controller
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
        private readonly IMemoryCache _cache;
        public HomeController(Context context
       , IUsersService applicationUserService
       , IOrderService orderService
       , IShoppingCartService shoppingCartService
       , IProductService productService
       , UserManager<IdentityUser> userManager
       , RoleManager<IdentityRole> roleManager
       , SignInManager<IdentityUser> signInManager
       , ILogger<HomeController> logger
       , IMemoryCache cache)
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
            _cache = cache;
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!_roleManager.RoleExistsAsync(ApplicationRoles.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Admin));
                await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.User));
            }
            var allProducts = _productService.GetAll();
            return View(allProducts);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(int productId)
        {
            var product = await _productService.FindFirstOrDefaultObjcetAsync(p=>p.id==productId);
            
            if (product != null)
            {
                var userId = _userManager.GetUserId(User);
                var user = _applicationUserService.FindFirstOrDefaultObjcet(u=>u.Id==userId);
                var allUserProductsInCart = _shoppingCartService.FindAllUserCarts(userId);
                var shoppingCartItemExist = allUserProductsInCart.FirstOrDefault(c => c.ProductId == productId);
                bool isSuccess = false;
                int shoppingCartCount = 0;
                if (shoppingCartItemExist == null)
                {
                    product.Size = "medium";
                    var shoppingCart = new ShoppingCartItem()
                    {
                        //وقتی کاربر از این حالت برای اضافه کردن محصول به کارت استفاده کرده یعنی یک عدد میخواهد
                        Count = 1,
                        ProductId = product.id,
                        UserId = userId
                    };
                    isSuccess = await _shoppingCartService.AddAsync(shoppingCart);
                }
                else
                {
                    shoppingCartCount = _shoppingCartService.increamentCount(shoppingCartItemExist, 1);
                }
                if (shoppingCartCount == -1)
                {
                    return StatusCode(404);
                }
            }
            return Redirect("/Main/Home/Index");
        }
        [HttpGet]
        public async Task<IActionResult> UserAccount()
        {
            if (_applicationUserService.IsSuperAdminExitst())
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = _applicationUserService.FindFirstOrDefaultObjcet(u => u.Id == userId);
                    var rolename = await _userManager.GetRolesAsync(user);
                    if (rolename.Contains("Superadmin") || rolename.Contains("Administrator"))
                    {
                        // User has the necessary roles, redirect to AdminAccount action
                        return RedirectToAction("AdminAccount", "Account", new { area = "Admin", user = JsonConvert.SerializeObject(user) });
                    }
                    else
                        return View(user);
                }
                else
                    return Redirect("/Identity/Login");
            }
            else
                return Redirect("/Admin/Account/LoginSuperAdmin");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInformation(User user)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = _applicationUserService.FindFirstOrDefaultObjcet(u => u.Id == userId);
            if (currentUser != null && currentUser.Id == user.Id)
            {
                _applicationUserService.Update(user);
                return Redirect("/Main/Home/UserAccount?userId=" + currentUser.Id.ToString());
            }
            return NotFound();
        }

    }
}
