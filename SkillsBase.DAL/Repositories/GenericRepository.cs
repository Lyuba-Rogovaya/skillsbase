using SkillsBase.DAL.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.DAL.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        protected SkillsBaseContext Context { get; set; }
        protected DbSet<TEntity> DbSet { get; set; }

        public GenericRepository(SkillsBaseContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }
        public virtual int Count () => DbSet.Count();
        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }
        public virtual Task<TEntity> GetByID(object id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return DbSet.FindAsync(id);
        }
        public virtual Task<TEntity> GetByID(params object[] keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            return DbSet.FindAsync(keyValues);
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Add(entity);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null)
                throw new ArgumentNullException(nameof(entityToDelete));

            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
                throw new ArgumentNullException(nameof(entityToUpdate));

            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
