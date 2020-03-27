using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TechnologyNews.Models;
using TechnologyNews.Data.Repositories;
using TechnologyNews.Services;
using TechnologyNews.Services.Interfaces;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace TechnologyNews.Data.Interfaces
{
    public interface IUnitOfWork
    {
        void MigrateDatabse(string targetMigration);
        static string HerokuPostgreSqlConnectionString { get; set; }
        string BaseUrl { get; set; }
        TechnologyNewsDbContext _context { get; set; }
        AboutRepository AboutRepository { get; }
        GenericRepository<Page> PageRepository { get; }
        GenericRepository<Post> PostRepository { get; }
        GenericRepository<Comment> CommentRepository { get; }
        GenericRepository<Contact> ContactRepository { get; }
        GenericRepository<Advertisement> AdvertisementRepository { get; }
        GenericRepository<CustomAnalytics> CustomAnalyticsRepository { get; }
        GenericRepository<SocialLinks> SocialLinksRepository { get; }
        IHttpContextAccessor httpContextAccessor { get; }
        UserManager<ApplicationUser> UserManager { get; }
        SignInManager<ApplicationUser> SignInManager { get; }
        UserService UserService { get; }
        Task<JObject> GetLocationAsync();
        string ClientIpAddress();
        string RandomColor();
        bool CreateDefaults();
        Task<int> SaveAsync();
        void Dispose();
    }
}
