using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UserToAdminVM
    {
        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        [MaxLength(50, ErrorMessage = "نام کاربری باید کمتر از 50 حرف باشد")]
        public string Username { get; set; }
        [Required(ErrorMessage = "لطفا ایمیل خود را انتخاب نمایید")]
        [EmailAddress(ErrorMessage = "مقدار وارد شده یک ایمیل نمیباشد")]
        public string Email { get; set; }
    }
}
