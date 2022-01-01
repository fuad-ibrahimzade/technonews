using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using TechnologyNews.Data.Configurations;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Models;
using TechnologyNews.Models.Interfaces;

namespace TechnologyNews.Data
{
    public class TechnologyNewsDbContext : IdentityDbContext<ApplicationUser>
    {
        public static string HerokuPostgreSqlConnectionString { get; set; }
        public TechnologyNewsDbContext()
        {
        }

        public TechnologyNewsDbContext (DbContextOptions<TechnologyNewsDbContext> options)
            : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((IEntity)entityEntry.Entity).date_upd = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((IEntity)entityEntry.Entity).date_add = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.HasDefaultSchema("public");//for postgre sql
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            //builder.ApplyConfiguration(new RoleConfiguration());
            //builder.Entity<User>().Property(x => x.UserName).IsRequired(required:false);
            //builder.Seed();

            builder.Entity<Comment>()
            .Property(e => e.child_comment_ids)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList<string>());
            //builder.Entity<CustomAnalytics>()
            //.Property(e => e.analytics_data)
            //.HasConversion(
            //    v => v.ToString(),
            //    v => JObject.Parse(v));
        }

        public DbSet<About> About { get; set; }
        public DbSet<Page> Page { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Advertisement> Advertisement { get; set; }
        public DbSet<SocialLinks> SocialLinks{ get; set; }
        public DbSet<CustomAnalytics> CustomAnalytics { get; set; }
        //public DbSet<User> User { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql(HerokuPostgreSqlConnectionString);
        //}

        public void MigrateDatabse(string targetMigration)
        {
            //if (!(_context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            //    _context.GetInfrastructure().GetService<IMigrator>().Migrate(targetMigration);
            this.GetInfrastructure().GetService<IMigrator>().Migrate(targetMigration);
        }
    }
}
