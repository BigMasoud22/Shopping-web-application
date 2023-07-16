using DataBase;
using DataBase.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ShopWeb.Areas.Admin.Controllers
{
    //در این کنترلر به جا دادن نقش ها به اتریبیوت آتورایز داخل اکشن ها به منظور جلوگیری از خطاهای احتمالی 
    //به صورت دستی در بعضی اکشن های مورد نیاز نقش کاربر داخل اکانت رو اعتبار سنجی میکنیم
    [Area("Admin")]
    public class AccountController : Controller
    {
        #region Services
        private readonly Context _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly IUsersService _usersService;
        private readonly IProductService _productService;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            RoleManager<IdentityRole> roleManager,
            Context context,
            IUsersService usersService,
            IProductService productService)
        {
            _rolemanager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _usersService = usersService;
            _productService = productService;
        }
        #endregion
        [BindProperty]
        public SuperAdminUserViewModel SuperAdmin { get; set; }
        [HttpGet]
        public async Task<IActionResult> LoginSuperAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginSuperAdmin(string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FullName = SuperAdmin.FullName;
                user.UserName = SuperAdmin.Username;
                user.Email = SuperAdmin.Email;
                var result = await _userManager.CreateAsync(user, SuperAdmin.Password);
                await _signInManager.SignInAsync(user, isPersistent: true);
                await _signInManager.PasswordSignInAsync(SuperAdmin.Email, SuperAdmin.Password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!_rolemanager.RoleExistsAsync(ApplicationRoles.superAdmin).GetAwaiter().GetResult())
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(ApplicationRoles.superAdmin));
                    }
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.superAdmin);
                    var userId = await _userManager.GetUserIdAsync(user);
                }
                return Redirect("/Main/Home/Index");
            }
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AdminAccount(string user)
        {
            var myDeserializedObject = JsonConvert.DeserializeObject<User>(user);
            // Perform authorization check here
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = _usersService.FindFirstOrDefaultObjcet(u => u.Id == userId);
            var rolename = await _userManager.GetRolesAsync(currentUser);
            myDeserializedObject.roleName = rolename.FirstOrDefault();
            if (!rolename.Contains("Superadmin") && !rolename.Contains("Administrator"))
            {
                return RedirectToAction("/Main/Home/UserAccount");
            }
            return View(myDeserializedObject);
        }
        #region Admins manage by superAdmin
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddNewAdmin()
        {
            var userId = _userManager.GetUserId(User);
            if (await IsSuperAdmin(userId))
            {
                SuperAdminUserViewModel adminVM = new SuperAdminUserViewModel();
                return View(adminVM);
            }
            return StatusCode(403);
        }
        [HttpPost]
        //به این دلیل از مدل سوپر ادمین استاده میکنیم که اطلاعات مورد نیازمون رو داره
        //و لارم به ساخت ویو مدل جدید نیست
        public async Task<IActionResult> AddNewAdmin(SuperAdminUserViewModel adminVM)
        {
            var isUserExist = _usersService.FindFirstOrDefaultObjcet(u => u.Email == adminVM.Email || u.UserName == adminVM.Username);
            if (isUserExist == null)
            {
                var user = CreateUser();
                user.FullName = adminVM.FullName;
                user.UserName = adminVM.Username;
                user.Email = adminVM.Email;
                var result = await _userManager.CreateAsync(user, adminVM.Password);
                await _signInManager.PasswordSignInAsync(adminVM.Email, adminVM.Password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
                    var userId = await _userManager.GetUserIdAsync(user);
                    TempData["AddAdminSuccess"] = "ادمین جدید با موفقیت اضافه شد";
                }
                else
                {
                    TempData["AddAdminFaild"] = "اضافه کردن ادمین جدید با شکست مواجه شد";
                }
                return Redirect("/Main/Home/UserAccount");
            }
            else
            {
                TempData["DuplicateAdmin"] = "یک ادمین با چنین مشخصاتی وجود دارد لطفا";
            }
            return View(adminVM);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GiveRoleToUsers()
        {
            var userId = _userManager.GetUserId(User);
            if (await IsSuperAdmin(userId))
            {
                UserToAdminVM adminVM = new UserToAdminVM();
                return View(adminVM);
            }
            return StatusCode(403);
        }
        [HttpPost]
        public async Task<IActionResult> GiveRoleToUsers(UserToAdminVM adminVM)
        {
            var user = _usersService.FindFirstOrDefaultObjcet(u => u.UserName == adminVM.Username && u.Email == adminVM.Email);
            if (user != null)
            {
                var entry = _context.Entry(user);
                if (entry.State == EntityState.Detached)
                {
                    _context.Users.Attach(user);
                }
                entry.State = EntityState.Modified;

                var success = await _userManager.RemoveFromRoleAsync(user, ApplicationRoles.User);
                if (success.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
                    TempData["AddAdminSuccess"] = "ادمین جدید با موفقیت اضافه شد";
                    return Redirect("/Main/Home/UserAccount");
                }
                return View();
            }
            else
            {
                TempData["AddAdminFaild"] = "کاربری با مشخصات وارد شده وجود ندارد";
                return View(adminVM);
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DemoteAdminToUser()
        {
            var userId = _userManager.GetUserId(User);
            if (await IsSuperAdmin(userId))
            {
                var user = new UserToAdminVM();
                return View(user);
            }
            return StatusCode(403);
        }
        [HttpPost]
        public async Task<IActionResult> DemoteAdminToUser(UserToAdminVM adminVM)
        {
            var user = _usersService.FindFirstOrDefaultObjcet(u => u.UserName == adminVM.Username && u.Email == adminVM.Email);
            var role =await _userManager.GetRolesAsync(user);
            if (user != null&&role.Contains(ApplicationRoles.Admin))
            {
                var entry = _context.Entry(user);
                if (entry.State == EntityState.Detached)
                {
                    _context.Users.Attach(user);
                }
                entry.State = EntityState.Modified;

                var success = await _userManager.RemoveFromRoleAsync(user, ApplicationRoles.Admin);
                if (success.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.User);
                    TempData["AddAdminSuccess"] = "نقش کاربر با موفقیت تغییر داده شد";
                    return Redirect("/Main/Home/UserAccount");
                }
                return View();
            }
            else
            {
                TempData["AddAdminFaild"] = "کاربری با مشخصات وارد شده وجود ندارد";
                return View(adminVM);
            }
        }
        #endregion
        [Authorize]
        public IActionResult LogoutAdmin()
        {
            var user = _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Json(new { success = false, message = "خطا در خروج..." });
            }
            _signInManager.SignOutAsync();
            return Json(new { success = true, meessage = "شما با موفقیت از حساب خود خارج شدید." });
        }
        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        private async Task<bool> IsSuperAdmin(string userId)
        {
            var user = _usersService.FindFirstOrDefaultObjcet(u => u.Id == userId);
            var rolename = await _userManager.GetRolesAsync(user);
            if (rolename.Contains("Superadmin"))
            {
                return true;
            }
            return false;
        }
    }
}
