using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TechnologyNews.Models.Interfaces;
using Newtonsoft.Json.Linq;

namespace TechnologyNews.Models
{
    public class CustomAnalytics : IEntity
    {
        public int id { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_upd { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_add { get; set; }
        public string analytics_type { get; set; }
        public string analytics_data { get; set; }
    }
}
