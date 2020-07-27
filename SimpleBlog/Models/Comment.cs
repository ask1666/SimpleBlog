using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public User Creator { get; set; }
        public Post Post { get; set; }
        public String Text { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; }
    }
}
