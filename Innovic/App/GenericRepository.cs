using Red.Wine;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Innovic.App
{
    public class GenericRepository<TEntity> : WineRepository<TEntity> where TEntity : WineModel
    {
        private readonly InnovicContext _context;
        private readonly string _userId;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(InnovicContext context, string userId) : base(context, userId)
        {
            _context = context;
            _userId = userId;
            _dbSet = _context.Set<TEntity>();
        }
    }
}