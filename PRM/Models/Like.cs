using System;
using System.Collections.Generic;

namespace PRM.Models
{
    public partial class Like
    {
        public int? UserId { get; set; }
        public int? VideoId { get; set; }

        public virtual User User { get; set; }
        public virtual Video Video { get; set; }
    }
}
