using Common.Models;
using Common.Models.Interfaces;

namespace Common.Helpers
{
    public static class IQueriableHelper
    {
        public static IQueryable<TEntity> GetUndeleted<TEntity>(this IQueryable<TEntity> query) where TEntity : class, IBaseEntity
        {
            return query.Where(e => e.DeleteDateTime == null);
        }
        public static IQueryable<TEntity> GetUnblocked<TEntity>(this IQueryable<TEntity> query, List<Guid> blocked) where TEntity : class, IBaseEntity, IHasUserId
        {
            return query.Where(e => !blocked.Contains(e.UserId));
        }
    }
}
