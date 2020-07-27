using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Models
{
    public class User : IdentityUser
    {
        public List<Comment> Comments { get; set; }
        public List<Post> Posts { get; set; }
    }
}
