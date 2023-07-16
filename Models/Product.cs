using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Product
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int SellCount { get; set; }
        public int AvailabilityCount { get; set; }
        public string? Size { get; set; }
        public string? MainImageAddress { get; set; }
        public string? picturesAlternativeText { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public virtual Category Category { get; set; }
        [ValidateNever]
        public virtual List<ImagesInformation>? Images { get; set; }
        [ValidateNever]
        public virtual List<Comments>? productComments { get; set; }

        public Product()
        {

        }
    }
}
