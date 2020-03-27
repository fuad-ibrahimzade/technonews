using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TechnologyNews.Models.Interfaces;

namespace TechnologyNews.Models
{
    public class Advertisement : IEntity
    {
        public int id { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_upd { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_add { get; set; }

        public string ads_name_1 { get; set; }
        public string ads_name_2 { get; set; }
        public string ads_picture_1 { get; set; }
        public string ads_picture_2 { get; set; }
    }
}
