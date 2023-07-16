using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class UserAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }
        [Required]
        public string City { get; set; }
        public string? State { get; set; }
        [Required(ErrorMessage ="کد پستی را پر کنید")]
        [MaxLength(10,ErrorMessage ="کد پستی نباید بیشتر از ده رقم باشد")]
        public string PostCode { get; set; }
        [Required(ErrorMessage ="آدرس را پر کنید")]
        public string Address { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
