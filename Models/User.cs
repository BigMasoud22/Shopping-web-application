using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        [NotMapped]
        public string roleName { get; set; }

        public int AddressId { get; set; }
        public UserAddress? Address { get; set; }
        public virtual List<Order>? Orders { get; set; }
        public virtual List<Comments>? userComments { get; set; }
    }
}
