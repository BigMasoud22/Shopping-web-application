using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class CartVM
    {
        [BindProperty]
        public List<ShoppingCartItem> carts { get; set; }
        public decimal lastPrice { get; set; }
    }
}
