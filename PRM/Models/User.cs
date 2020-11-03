using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PRM.Models
{
    public partial class User
    {
        public User()
        {
            Likes = new HashSet<Like>();
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public DateTime? CreationDate { get; set; }

        public virtual Video Video { get; set; }

        [JsonIgnore]
        public ICollection<Like> Likes { get; set; }
        [JsonIgnore]

        public ICollection<Comment> Comments { get; set; }
    }
}
