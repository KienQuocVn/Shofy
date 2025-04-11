using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shofy.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }


        [Required, StringLength(50), MinLength(3)]
        public string Username { get; set; }


        [Required, StringLength(256)]
        public string PasswordHash { get; set; }


        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }


        public string Role { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
