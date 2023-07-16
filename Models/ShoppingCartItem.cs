using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int Count { get; set; }

        [NotMapped]
        public decimal? totalPrice { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}