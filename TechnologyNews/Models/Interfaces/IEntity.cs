using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechnologyNews.Models.Interfaces
{
    public interface IEntity
    {
        public int id { get; set; }
        public DateTime date_add { get; set; }
        public DateTime date_upd { get; set; }
    }
}
