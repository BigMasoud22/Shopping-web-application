using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class SuperAdminUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [ValidateNever]
        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "مقدار وارد شده با رمز عبور شما همخوانی ندارد")]
        [ValidateNever]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        [MaxLength(50, ErrorMessage = "نام کاربری باید کمتر از 50 حرف باشد")]
        public string Username { get; set; }

        [Required(ErrorMessage = "نام و نام خانوداگی باید انتخاب بشود")]
        [MaxLength(50, ErrorMessage = "حداکثر طول 50 کاراکتر")]
        public string FullName { get; set; }
    }
}
