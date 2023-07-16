using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ImagesInformation
    {
        [Key]
        public int Id { get; set; }
        public string imageAddress { get; set; }

        [ForeignKey("product")]
        public int productId { get; set; }
        [ValidateNever]
        public virtual Product product { get; set; }
    }
}
