using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Models.Interfaces;

namespace TechnologyNews.Models
{
    public class Comment : IEntity
    {
        public int id { get; set; }
        public DateTime date_add { get; set; }
        public DateTime date_upd { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string message { get; set; }
        public int post_id { get; set; }
        public string parent_comment_id { get; set; }
        public List<string> child_comment_ids { get; set; }
    }
}
