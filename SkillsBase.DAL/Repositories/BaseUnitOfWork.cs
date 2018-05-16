using SkillsBase.DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.DAL.Repositories
{
    public class BaseUnitOfWork : IDisposable
    {
        protected SkillsBaseContext db;

        public BaseUnitOfWork(SkillsBaseContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            db = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }
        public virtual async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }
        public virtual void Save()
        {
            db.SaveChanges();
        }
    }
}
