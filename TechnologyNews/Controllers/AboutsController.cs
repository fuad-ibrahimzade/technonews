using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnologyNews.Data;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Models;
using TechnologyNews.Models.Interfaces;
using TechnologyNews.Data.Repositories;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using TechnologyNews.Helpers;

namespace TechnologyNews.Controllers
{

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AboutsController : ControllerBase, IDisposable
    {
        private IUnitOfWork _unitOfWork;

        public IUnitOfWork UnitOfWork => _unitOfWork;

        //private async void InitializeUnitOfWork()
        //{
        //    _unitOfWork = await UnitOfWork.Create();
        //}
        public AboutsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Abouts/CreateDefault
        [HttpGet("/api/Abouts/CreateDefault")]
        public IActionResult CreateDefaultAsync()
        {
            var databaseUrl = "postgres://yedmtmlmsqdltv:8885c79d1322620bcd8affc3a474c9bd82089a75cb148fff85893bc0005a87b6@ec2-46-137-84-140.eu-west-1.compute.amazonaws.com:5432/d3cbgi217b7ap6";
            var connectionString = "postgres://yedmtmlmsqdltv:8885c79d1322620bcd8affc3a474c9bd82089a75cb148fff85893bc0005a87b6@ec2-46-137-84-140.eu-west-1.compute.amazonaws.com:5432/d3cbgi217b7ap6";

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            if (TechnologyNewsDbContext.HerokuPostgreSqlConnectionString != null)
            {

                //"User ID =postgres;Password=1234;Server=localhost;Port=5432;Database=testDb; Integrated Security = true; Pooling = true; 
                //"DefaultConnection": "Server=localhost;Port=5432;User Id=username;Password=secret;Database=todos;"
                string herokuConnectionString = $@"
                  Host={databaseUrl};
                  Port=<port>;
                  Username=<user>;
                  Password=<password>;
                  Database=<database>;
                  Pooling=true;
                  Use SSL Stream=True;
                  SSL Mode=Require;
                  TrustServerCertificate=True;
                ";
                var builder = new Npgsql.NpgsqlConnectionStringBuilder
                {
                    Host = databaseUri.Host,
                    Port = databaseUri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = databaseUri.LocalPath.TrimStart('/'),
                    SslMode = Npgsql.SslMode.Prefer,
                    TrustServerCertificate = true
                };
                string tempURL = "https://res.cloudinary.com/dfebwmqmq/image/upload/v1573550127/lmhzv9dpiq3ducxykow9.jpg";
                string publicId = Regex.Replace(tempURL, $@"{(tempURL.Contains("https") ? "https" : "http")}:\/\/res\.cloudinary\.com\/.*\/image\/upload\/", "");
                //publicId = Regex.Replace(tempURL, $@"{(tempURL.Contains("https") ? "https" : "http")}", "");
                var locationInfoJson = _unitOfWork.GetLocationAsync().GetAwaiter().GetResult().ToString();
                //var featuredPosts = _unitOfWork.PostRepository.context.Post.FromSqlRaw("SELECT * FROM {0} ORDER BY RANDOM() LIMIT {1};", "Post",2).ToList();
                var tableNamesQuery = @"
                    SELECT table_name
                    FROM information_schema.tables
                    WHERE table_schema='public'
                    AND table_type='BASE TABLE';
                ";
                List<string> tableNamesQueryResult = new List<string>();
                //var tableNames = _unitOfWork._context.Database.ExecuteSqlRaw(tableNamesQuery);
                //var tableNames = _unitOfWork._context.Set<dynamic>().FromSqlRaw(tableNamesQuery);
                using (var context = new TechnologyNewsDbContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = tableNamesQuery;
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        int count = result.FieldCount;
                        while (result.Read())
                        {
                            for (int i = 0; i < count; i++)
                            {
                                tableNamesQueryResult.Add(result.GetString(i));
                            }
                        }

                        //result.Close();
                    }
                }

                return Ok(new
                {
                    publicId,
                    tempURL,
                    databaseUrl,
                    connectionString,
                    builder = builder.ToString(),
                    tableNamesQueryResult
                });
            }
            return Ok();
            //var key = new byte[32];
            //using (var generator = System.Security.Cryptography.RandomNumberGenerator.Create())
            //    generator.GetBytes(key);
            //string apiSecret = Convert.ToBase64String(key);
            //return Ok(apiSecret);
            //await UnitOfWork.UserManager.CreateAsync(new ApplicationUser
            //{
            //    ApiToken = "some",
            //    UserName = "admin@admin.com",
            //    Email = "admin@admin.com",
            //    EmailConfirmed = true,
            //    DateAdd = DateTime.Now,
            //    DateUpd = DateTime.Now,
            //}, "admin");
            //var returnObject = string.Format("{0}://{1}{2}", HttpContext.Request.Scheme, HttpContext.Request.Host, Url.Content("~"));
            ////return await UnitOfWork.AboutRepository.GetAllAsync();
            ////return Ok(await UnitOfWork.UserManager.Users.ToListAsync());
            //return Ok(returnObject);
        }

        // GET: api/Abouts
        [HttpGet]
        public async Task<IEnumerable<About>> GetAboutAsync()
        {
            return await UnitOfWork.AboutRepository.GetAllAsync();
        }

        // GET: api/Abouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<About>> GetAbout(int id)
        {
            var about = await UnitOfWork.AboutRepository.GetByIDAsync(id);

            if (about == null)
            {
                return NotFound();
            }

            return about;
        }

        // PUT: api/Abouts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAbout(int id,[FromBody] About about)
        {
            if (id != about.id)
            {
                return BadRequest();
            }

            try
            {
                UnitOfWork.AboutRepository.Update(about);
                return Ok(await UnitOfWork.SaveAsync());
            }
            catch (Exception)
            {
                Boolean aboutExist = (await UnitOfWork.AboutRepository.GetByIDAsync(about.id)) != null;
                if (!aboutExist)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Abouts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> PostAbout(About about)
        {
            UnitOfWork.AboutRepository.Insert(about);
            return Ok(await UnitOfWork.SaveAsync());

            //return CreatedAtAction("GetAbout", new { id = about.Id }, about);
        }

        // DELETE: api/Abouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbout(int id)
        {
            var about = await UnitOfWork.AboutRepository.GetByIDAsync(id);
            if (about == null)
            {
                return NotFound();
            }

            UnitOfWork.AboutRepository.Delete(about);
            await UnitOfWork.SaveAsync();
            return Ok(await UnitOfWork.SaveAsync());
        }

        private async Task<bool> AboutExistsAsync(int id)
        {
            //return _context.About.Any(e => e.Id == id);
            return await UnitOfWork.AboutRepository.GetByIDAsync(id) != null;
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
