using System;
using System.Collections.Generic;

namespace PRM.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
