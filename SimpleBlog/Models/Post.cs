using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Models
{
    public class Post
    {
        public int ID { get; set; }
        public User Creator { get; set; }
        public String Title { get; set; }
        public String Text { get; set; }
        public List<Comment> Comments { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedTime { get; set; }
    }
}
