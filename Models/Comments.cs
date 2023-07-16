using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }
        public string Body { get; set; }

        [ForeignKey("products")]
        public int ProductId { get; set; }
        public virtual Product products { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

    }
}
