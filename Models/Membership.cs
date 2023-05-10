using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YRPortal.Models
{
    public class Membership
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Username should be less than 50")]
        public string Username { get; set; }
        [Required]
        [StringLength(50 , ErrorMessage = "Password should be less than 50")]
        public string Password { get; set; }
    }
}