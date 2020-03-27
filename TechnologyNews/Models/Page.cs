using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Models.Interfaces;

namespace TechnologyNews.Models
{
    public class Page : IEntity
    {
        public int id { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_upd { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_add { get; set; }

        public int posts_count { get; set; }
        public string name { get; set; }
        public string category_color { get; set; }
        public bool show_at_home { get; set; }
        public int showing_order { get; set; }
    }
}
