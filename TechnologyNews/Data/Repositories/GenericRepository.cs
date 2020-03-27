using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TechnologyNews.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechnologyNews.Data;
using System.Threading.Tasks;
using TechnologyNews.Models.Interfaces;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Data.Repositories.Interfaces;
using System.Linq.Dynamic.Core;

namespace TechnologyNews.Data.Repositories
{
    public class GenericRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IEntity
    {
        internal TechnologyNewsDbContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(TechnologyNewsDbContext context)
        {
            //, [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIDAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<bool> IsEmpty()
        {
            return !await dbSet.AnyAsync();
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
            //await SaveAsync();
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            //await SaveAsync();
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.FindAsync(id).GetAwaiter().GetResult();
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            //await SaveAsync();
        }

        public IEnumerable<object> GetRandomLimit(int limit)
        {
            var entities = dbSet.AsEnumerable<TEntity>();
            Random rnd = new Random();
            return entities.OrderBy(r => rnd.Next()).Take(limit).ToList();
            //$db = static::getDB();
            //$stmt = null;
            //try
            //{
            //// $stmt = $db->query('SELECT * FROM posts ORDER BY rand() LIMIT '.$limit);//mysql
            //$stmt = $db->query('SELECT * FROM posts ORDER BY random() LIMIT '.$limit);
            //    return $stmt->fetchAll(PDO::FETCH_OBJ);
            //}
            //catch (\Exception $e){
            //    //    echo 'Caught exception: ',  $e->getMessage(), "\n";
            //}
            //return false;


            //string randomsQuery = $"SELECT * FROM public.\"{typeof(TEntity).Name}\" ORDER BY RANDOM() LIMIT {limit};";
            //List<object> randomsQueryResult = new List<object>();
            //using (var command = context.Database.GetDbConnection().CreateCommand())
            //{
            //    command.CommandText = randomsQuery;
            //    context.Database.OpenConnection();
            //    using (var result = command.ExecuteReader())
            //    {
            //        //DataTable schemaTable = result.GetSchemaTable();
            //        //randomsQueryResult = result.AsQueryable().Cast<TEntity>().ToList<TEntity>();
            //        DataTable schemaTable = result.GetSchemaTable();

            //        foreach (DataRow row in schemaTable.Rows)
            //        {
            //            foreach (DataColumn column in schemaTable.Columns)
            //            {
            //                randomsQueryResult.Add(String.Format("{0} = {1}",
            //                   column.ColumnName, row[column]));
            //            }
            //        }

            //        //int count = result.FieldCount;
            //        //while (result.Read())
            //        //{
            //        //    object[] values = new object[typeof(TEntity).GetProperties().Length];
            //        //    var fieldCount = result.GetValues(values);
            //        //    //randomsQueryResult.Add(values.Cast<object>());
            //        //    List<object> entity = new List<object>();
            //        //    //for (int i = 0; i < fieldCount; i++)
            //        //    //    randomsQueryResult.Add(values[i]);
            //        //    //Console.WriteLine(values[i]);

            //        //    //randomsQueryResult.Add(result.Cast<object>().FirstOrDefault());
            //        //    //randomsQueryResult.Add(result.Cast<TEntity>().FirstOrDefault());
            //        //    for (int i = 0; i < fieldCount; i++)
            //        //    {
            //        //        entity.Add(result.GetFieldValue<object>(i));
            //        //        //result.GetValues(entity);
            //        //        //randomsQueryResult.Add(result.Cast<TEntity>().FirstOrDefault())
            //        //        //randomsQueryResult.Add(result.GetFieldValue<object>(i));
            //        //    }
            //        //    randomsQueryResult.Add(entity.Cast<TEntity>().FirstOrDefault());
            //        //}

            //        //result.Close();
            //    }
            //}



            //TEntity[] b = randomsQueryResult as TEntity[];
            //return await dbSet.OrderBy(r => Guid.NewGuid()).Take(limit).ToListAsync();
            //return await dbSet.FromSqlInterpolated($"SELECT * FROM {typeof(TEntity).Name} ORDER BY RANDOM() LIMIT {limit};").ToListAsync();
            //return randomsQueryResult;
            //return dbSet.FromSqlInterpolated($"SELECT * FROM public.\"{typeof(TEntity).Name}\" ORDER BY RANDOM() LIMIT {limit};").ToList();

        }

        public async Task<object> GetDistinctColumnAsync(string column)
        {
            var columnData = await dbSet.Select($"new {typeof(TEntity).GetType().ToString()} {{ {column} = p.{column} }}").ToDynamicListAsync();
            return columnData;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
