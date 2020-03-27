using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Data;
using TechnologyNews.Models;
using TechnologyNews.Data.Repositories.Interfaces;

namespace TechnologyNews.Data.Repositories
{
    //public class UserRepository : GenericRepository<User>, IUserRepository
    //{
    //    public UserRepository(TechnologyNewsDbContext context) : base(context)
    //    {
    //    }

    //    public async Task<IEnumerable<User>> GetUserByEmail(string email)
    //    {
    //        //from user in base.context.User where user.Email == email select user
    //        //IQueryable<User> scoreQuery =
    //        //        from u in context.User
    //        //        where u.Email == email
    //        //        select u;
    //    //filter: user => user.Email == email
    //        return await base.GetAsync(filter: user => user.Email == email);
    //    }
    //}
}
