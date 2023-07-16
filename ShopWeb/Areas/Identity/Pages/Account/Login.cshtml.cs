// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Models;
using DataBase.Services.IServices;
using DataBase.Services;

namespace ShopWeb.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        #region Services
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly IUsersService _usersService;
        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUsersService usersService)
        {
            _rolemanager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<IdentityUser>)GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
            _usersService = usersService;
        }
        #endregion
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [ValidateNever]
            [Required(ErrorMessage = "لطفا ایمیل خود را انتخاب نمایید")]
            [EmailAddress(ErrorMessage = "مقدار وارد شده یک ایمیل نمیباشد")]
            public string Email { get; set; }
            [Required(ErrorMessage = "لطفا رمز خود را وارد نمایید")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [ValidateNever]
            [Display(Name = "مرا به خاطر بسپار")]
            public bool RememberMe { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تایید رمز")]
            [ValidateNever]
            [Compare("Password", ErrorMessage = "مقدار وارد شده با رمز عبور شما همخوانی ندارد")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
            [MaxLength(50, ErrorMessage = "نام کاربری باید کمتر از 50 حرف باشد")]
            public string Username { get; set; }

            [ValidateNever]
            public bool IsLogin { get; set; }

            [ValidateNever]
            [Required(ErrorMessage = "لطفا نام و نام خانوادگی خود را وارد نمایید")]
            [MaxLength(50, ErrorMessage = "حداکثر طول 50 کاراکتر")]
            public string FullName { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (_usersService.IsSuperAdminExitst())
            {
                if (!_rolemanager.RoleExistsAsync(ApplicationRoles.Admin).GetAwaiter().GetResult())
                {
                    _rolemanager.CreateAsync(new IdentityRole(ApplicationRoles.User)).GetAwaiter().GetResult();
                }
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ModelState.AddModelError(string.Empty, ErrorMessage);
                }
                returnUrl ??= Url.Content("~/");
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                ReturnUrl = returnUrl;
                return Page();
            }
            else
            {
                return Redirect("/Admin/Account/LoginSuperAdmin");
            }
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (Input.IsLogin)
                {
                    var username = Input.Username;
                    var password = Input.Password;
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return Redirect(returnUrl ?? "/");
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                else
                {
                    var usernameExist = _usersService.FindFirstOrDefaultObjcet(u => u.UserName == Input.Username||u.Email==Input.Email);
                    if (usernameExist==null)
                    {
                        var user = CreateUser();
                        user.FullName = Input.FullName;
                        user.UserName = Input.Username;
                        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                        var result = await _userManager.CreateAsync(user, Input.Password);


                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User created a new account with password.");

                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            await _rolemanager.CreateAsync(new IdentityRole(ApplicationRoles.User));
                            await _userManager.AddToRoleAsync(user, ApplicationRoles.User);

                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                                protocol: Request.Scheme);

                            //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                            }
                            else
                            {
                                await _signInManager.SignInAsync(user, isPersistent: true);
                                return Redirect(returnUrl);
                            }
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Input.Username", "این نام کاربری یا ایمیل توسط شخص دیگری انتخاب شده است لطفا نام کاربری یا ایمیل دیگری انتخاب کنید");
                    }
                }
            }
            
            // If we got this far, something failed, redisplay form
            return Page();
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
        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
