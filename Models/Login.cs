//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YRPortal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Login
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Username should be less than 50")]
        public string Username { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Password should be less than 50")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
