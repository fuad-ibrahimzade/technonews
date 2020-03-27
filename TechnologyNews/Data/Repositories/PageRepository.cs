using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Data;
using TechnologyNews.Models;

namespace TechnologyNews.Data.Repositories
{
    public class PageRepository : GenericRepository<Page>
    {
        public PageRepository(TechnologyNewsDbContext context) : base(context)
        {
        }
    }
}
