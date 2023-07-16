using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "گذرواژه قدیمی ضروری است")]
        [DataType(DataType.Password)]
        [Display(Name = "گذرواژه فعلی")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "گذرواژه جدید ضروری است")]
        [StringLength(100, ErrorMessage = "گذرواژه جدید باید حداقل 6 کاراکتر و حداکثر 100 کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "گذرواژه جدید")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تایید گذرواژه جدید ضروری است")]
        [DataType(DataType.Password)]
        [Display(Name = "تایید گذرواژه جدید")]
        [Compare("NewPassword", ErrorMessage = "گذرواژه جدید و مقدار وارد شده همخوانی ندارند")]
        public string ConfirmPassword { get; set; }
    }
}
