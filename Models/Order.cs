using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public int OrderTotal { get; set; }
        public string ProductColor { get; set; }
        public string ProductSize { get; set; }
        public string State { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [ValidateNever]
        public virtual User User { get; set; }
        [ValidateNever]
        public virtual IEnumerable<ShoppingCartItem>? Cart { get; set; }
    }
}