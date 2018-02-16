using Innovic.Models;
using Innovic.Models.Sales;
using Red.Wine;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Innovic.Services
{
    public class BaseService<TEntity> where TEntity : BaseModel
    {
        private static InnovicContext _context = new InnovicContext();
        private DbSet<TEntity> _db = _context.Set<TEntity>();

        public virtual bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return _db.Count(filter) > 0;
        }

        public virtual TEntity QuickCreate(TEntity entity)
        {
            return _db.Add(entity);
        }

        public virtual TEntity QuickCreateAndSave(TEntity entity)
        {
            var addentity = _db.Add(entity);
            _context.SaveChanges();
            return addentity;
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> filter = null)
        {
            return _db.Where(filter).SingleOrDefault();
        }

        public virtual TEntity Process(TEntity entity)
        {
            return null;
        }

    }
}