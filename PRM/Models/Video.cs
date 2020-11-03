using System;
using System.Collections.Generic;

namespace PRM.Models
{
    public partial class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public int? UserId { get; set; }
        public string UrlShare { get; set; }

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        public virtual User user { get; set; }

        public ICollection<Like> Likes { get; set; }

        public ICollection<Comment> Comments { get; set; }

    }
}
