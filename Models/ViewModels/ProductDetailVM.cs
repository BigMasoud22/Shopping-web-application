using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ProductDetailVM
    {
        [BindProperty]
        public Comments comments { get; set; }

        public int countCart { get; set; }

        public Product product { get; set; }

        public List<Comments> productCommnets { get; set; }
        public List<Product> relatedProducts { get; set; }
    }
}
