using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Lotus.Core;

namespace Lotus.Data
{
    public interface IDbContext : IDisposable
    {
        XResult<Boolean> Add<TEntity>(TEntity entity) where TEntity : class;
        XResult<Boolean> AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        IEnumerable<TEntity> Find<TEntity>(Expression<Func<TEntity, Boolean>> filter) where TEntity : class;
        Task<IEnumerable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, Boolean>> filter, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
        XResult<Boolean> Remove<TEntity>(TEntity entity) where TEntity : class;
        XResult<Boolean> RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        XResult<Boolean> Update<TEntity>(TEntity entity) where TEntity : class;
        XResult<Boolean> UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        XResult<Int32> SaveChange(Boolean useTransaction = false);
        Task<XResult<Int32>> SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken), Boolean useTransaction = false);
        IQueryable<TEntity> AsQueryable<TEntity>();
    }
}
