using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ProductTblVM
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int sellCount { get; set; }
        public bool IsAvaliable { get; set; }
        public string CategoryName { get; set; }
        public decimal price { get; set; }
        public string Alt { get; set; }

        public static List<ProductTblVM> tblVM(List<Product> allProducts)
        {
            List<ProductTblVM> products = allProducts.Select(p => new ProductTblVM
            {
                id = p.id,
                price = p.Price,
                Name = p.Name,
                sellCount = p.SellCount,
                IsAvaliable = p.IsAvailable,
                CategoryName = p.Category.Name,
                Alt =p.picturesAlternativeText
            }).ToList();
            return products;
        }
    }
}
