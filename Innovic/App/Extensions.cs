using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace Innovic.App
{
    public static class Extensions
    {
        public static List<TEntity> GetLocalEntities<TEntity>(this List<TEntity> entities) where TEntity : BaseModel
        {
            if (entities.Count == 0)
                return entities;

            var context = GetContext(entities);
            return entities.Where(e => context.Entry(e).State == EntityState.Detached || context.Entry(e).State == EntityState.Added).Select(e => e).ToList();
        }

        public static List<TEntity> GetDatabaseEntities<TEntity>(this List<TEntity> entities) where TEntity : BaseModel
        {
            return entities.Except(GetLocalEntities(entities)).ToList();
        }

        public static DbContext GetContext<TEntity>(this List<TEntity> entities) where TEntity : BaseModel
        {
            var objectContext = GetObjectContextFromEntity(entities[0]);

            if (objectContext == null)
                return null;

            return new DbContext(objectContext, dbContextOwnsObjectContext: false);
        }

        public static DbContext GetContext(this BaseModel entity)
        {
            var objectContext = GetObjectContextFromEntity(entity);

            if (objectContext == null)
                return null;

            return new DbContext(objectContext, dbContextOwnsObjectContext: false);
        }

        private static ObjectContext GetObjectContextFromEntity(this BaseModel entity)
        {
            var field = entity.GetType().GetField("_entityWrapper");

            if (field == null)
                return null;

            var wrapper = field.GetValue(entity);
            var property = wrapper.GetType().GetProperty("Context");
            var context = (ObjectContext)property.GetValue(wrapper, null);

            return context;
        }
    }
}