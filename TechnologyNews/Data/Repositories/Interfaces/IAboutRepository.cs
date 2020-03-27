using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Models;

namespace TechnologyNews.Data.Repositories.Interfaces
{
    public interface IAboutRepository : IBaseRepository<About>
    {
        Task<IEnumerable<About>> CreateDefault();
    }
}
