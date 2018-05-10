using System.Collections.Generic;
using System.Linq;
using HoveyTech.Core.Contracts.Data;
using HoveyTech.Core.Contracts.Model;
using HoveyTech.Core.Data.EntityFrameworkCore.Base;
using HoveyTech.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace HoveyTech.Core.Data.EntityFrameworkCore.Extensions
{
    public static class RepositoryExtensions
    {
        public static bool DoesEntityExistById<TEntity>(this IHasTransactionRepository<TEntity> repository, object id)
            where TEntity : class
        {
            using (var tran = (Transaction)repository.GetTransaction())
            {
                var entity = tran.GetSet<TEntity>().Find(id);
                var exists = entity != null;

                if (exists)
                {
                    var entry = tran.Context.Entry(entity);
                    if (entry.State == EntityState.Unchanged)
                        entry.State = EntityState.Detached;
                }

                tran.CommitIfOwner();

                return exists;
            }
        }
        
        public static IList<TEntity> GetAllActiveSorted<TEntity>(this IHasTransactionRepository<TEntity> repository, int? optionalId = null)
              where TEntity : BaseEntityWithIntKey, IIsActive, INamedEntity, new()
        {
            using (var tran = (Transaction)repository.GetTransaction())
            {
                IQueryable<TEntity> query = tran.GetSet<TEntity>();

                if (optionalId.HasValue)
                    query = query.Where(x => x.IsActive || x.Id == optionalId.Value);
                else
                    query = query.Where(x => x.IsActive);

                query = query.OrderBy(x => x.Name);

                var results = query.ToList();

                tran.CommitIfOwner();

                return results;
            }
        }

        public static IList<TEntity> GetAllActive<TEntity>(this IHasTransactionRepository<TEntity> repository)
            where TEntity : class, IIsActive
        {
            using (var tran = (Transaction)repository.GetTransaction())
            {
                IQueryable<TEntity> query = tran.GetSet<TEntity>()
                    .Where(x => x.IsActive);

                var results = query.ToList();

                tran.CommitIfOwner();

                return results;
            }
        }

        public static IList<TEntity> GetAllActive<TEntity>(this IHasTransactionRepository<TEntity> repository, int? optionalId = null)
            where TEntity : BaseEntityWithIntKey, IIsActive, new()
        {
            using (var tran = (Transaction)repository.GetTransaction())
            {
                IQueryable<TEntity> query = tran.GetSet<TEntity>();

                if (optionalId.HasValue)
                    query = query.Where(x => x.IsActive || x.Id == optionalId.Value);
                else
                    query = query.Where(x => x.IsActive);

                var results = query.ToList();

                tran.CommitIfOwner();

                return results;
            }
        }

        public static IList<TEntity> GetAllSorted<TEntity>(this IHasTransactionRepository<TEntity> repository)
            where TEntity : class, INamedEntity
        {
            using (var tran = (Transaction)repository.GetTransaction())
            {
                var results = tran.GetSet<TEntity>()
                    .OrderBy(x => x.Name)
                    .ToList();
                
                tran.CommitIfOwner();

                return results;
            }
        }
    }
}
