﻿using System;
using System.Collections.Generic;

namespace PRM.Models
{
    public partial class Comment
    {
        public int? UserId { get; set; }
        public int? VideoId { get; set; }
        public string Conttent { get; set; }

        public virtual User User { get; set; }
        public virtual Video Video { get; set; }
    }
}