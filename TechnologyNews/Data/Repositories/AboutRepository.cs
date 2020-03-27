using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Data;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Models;
using TechnologyNews.Models.Interfaces;
using TechnologyNews.Data.Repositories.Interfaces;

namespace TechnologyNews.Data.Repositories
{
    public class AboutRepository : GenericRepository<About>, IAboutRepository
    {
        public AboutRepository(TechnologyNewsDbContext context) : base(context)
        {

        }

        public Task<IEnumerable<About>> CreateDefault()
        {
            throw new NotImplementedException();
        }

    }
}
