using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Models.Interfaces;

namespace TechnologyNews.Models
{
    public class Post : IEntity
    {
        public int id { get; set; }
        public DateTime date_add { get; set; }
        public DateTime date_upd { get; set; }
        public string post_category { get; set; }
        public string post_title { get; set; }
        public string post_tags { get; set; }
        public string post_picture { get; set; }
        public string content { get; set; }
        public string post_desc { get; set; }
        public string author_name { get; set; }
        public string author_desc { get; set; }
        public string author_picture { get; set; }
        public string author_social_links { get; set; }
    }
}
