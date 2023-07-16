using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UpsertProductVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام محصول اجباریست")]
        [MaxLength(100, ErrorMessage = "حداکثر 100 کاراکتر")]
        [DisplayName("نام محصول")]
        public string Name { get; set; }

        [Required(ErrorMessage = "توضیح کامل محصول اجباریست")]
        [MaxLength(200, ErrorMessage = "حداکثر 300 کاراکتر")]
        [DisplayName("توضیح محصول")]
        public string Desctioption { get; set; }

        [Required(ErrorMessage = "توضیح مختصر محصول اجباریست")]
        [MaxLength(200, ErrorMessage = "حداکثر 100 کاراکتر")]
        [DisplayName("توضیح محصول")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "قیمت اجباریست")]
        [DisplayName("قیمت")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "تعداد موجود محصول اجباریست")]
        public int AvailabilityCount { get; set; }

        [Required(ErrorMessage = "حداقل یک شایز باید وجود داشته باشد")]
        public string? Size { get; set; }

        [Required(ErrorMessage = "تایتل تصاویر محصول اجباریست")]
        [MaxLength(100, ErrorMessage = "بیشتر از مقدار وارد شده در دسترس نیست")]
        public string? Alt { get; set; }

        public string? MainPhotoaddress { get; set; }

        [Required(ErrorMessage = "لطفا دسته را انتخاب کنید")]
        public int CategoryId { get; set; }
        public static Product MergeProductVMtoProduct(UpsertProductVM productVM)
        {
            var product = new Product
            {
                id = productVM.Id,
                Name = productVM.Name,
                Description = productVM.Desctioption,
                Price = productVM.Price,
                CategoryId = productVM.CategoryId,
                AvailabilityCount = productVM.AvailabilityCount,
                Size = productVM.Size,
                picturesAlternativeText = productVM.Alt,
                MainImageAddress = productVM.MainPhotoaddress,
                ShortDescription = productVM.ShortDescription
            };
            if (product.AvailabilityCount > 0)
            {
                product.IsAvailable = true;
            }
            return product;
        }
        public static UpsertProductVM MergeProducttoProductVM(Models.Product product)
        {
            return new UpsertProductVM
            {
                Alt = product.picturesAlternativeText,
                Price = (int)product.Price,
                Desctioption = product.Description,
                Name = product.Name,
                Id = product.id,
                CategoryId = product.CategoryId,
                AvailabilityCount = product.AvailabilityCount,
                MainPhotoaddress = product.MainImageAddress,
                Size = product.Size,
                ShortDescription = product.ShortDescription,
            };
        }
    }
}
