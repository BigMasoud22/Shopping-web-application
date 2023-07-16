using DataBase;
using DataBase.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace ShopWeb.Areas.Main.Controllers
{
    [Area("Main")]
    public class OperationController : Controller
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
        public OperationController(Context context
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
        public async Task<IActionResult> SetUserAddress(UserAddress address)
        {
            if (address!=null)
            {
                var findeUser = await _userManager.GetUserAsync(User);
                var theUser = _applicationUserService.FindFirstOrDefaultObjcet(u => u.Id == findeUser.Id);

                _applicationUserService.Update(theUser, address);
                return Redirect("/Main/Home/UserAccount");
            }
            return BadRequest();
        }

        [BindProperty]
        public ChangePasswordVM Input { get; set; }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string? url)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {

                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            TempData["ChangePasswordSuccessNotification"] = "گذرواژه با موفقیت تغییر یافت";
            return Redirect("/Main/Home/Index");
        }
    }
}
