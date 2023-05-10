using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YRPortal.Models
{
    public class CreateUser
    {

        public int ID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string role { get; set; }
        public string description { get; set; }
    }
}