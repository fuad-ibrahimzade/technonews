using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TechnologyNews.Models.Interfaces;

namespace TechnologyNews.Models
{
    public class About : IEntity
    {
        public int id { get; set; }
        public string content { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_upd { get; set; }

        [DataType(DataType.Date)]
        public DateTime date_add { get; set; }
    }
}
