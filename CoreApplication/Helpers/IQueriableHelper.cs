using CoreApplication.Models;

namespace CoreApplication.Helpers
{
    public static class IQueriableHelper
    {
        public static IQueryable<TEntity> GetUndeleted<TEntity>(this IQueryable<TEntity> query) where TEntity : class, IBaseEntity
        {
            return query.Where(e => e.DeleteDateTime == null);
        }
    }
}
