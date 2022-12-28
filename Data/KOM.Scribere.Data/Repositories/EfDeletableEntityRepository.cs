using KOM.Scribere.Data.Common.Repositories;

namespace KOM.Scribere.Data.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KingsOfMarketing.iShopper.Data.Common.Repositories;
using KOM.Scribere.Data.Common.Models;
using Microsoft.EntityFrameworkCore;

public class EfDeletableEntityRepository<TEntity> : EfRepository<TEntity>, IDeletableEntityRepository<TEntity>
    where TEntity : class, IDeletableEntity
{
    public EfDeletableEntityRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override IQueryable<TEntity> All() => base.All().Where(x => !x.IsDeleted);

    public override IQueryable<TEntity> AllAsNoTracking() => base.AllAsNoTracking().Where(x => !x.IsDeleted);

    public IQueryable<TEntity> AllWithDeleted() => base.All().IgnoreQueryFilters();

    public IQueryable<TEntity> AllAsNoTrackingWithDeleted() => base.AllAsNoTracking().IgnoreQueryFilters();

    public Task<TEntity> GetByIdWithDeletedAsync(params object[] id)
    {
        var getByIdPredicate = EfExpressionHelper.BuildByIdPredicate<TEntity>(this.Context, id);
        return this.AllWithDeleted().FirstOrDefaultAsync(getByIdPredicate);
    }

    public void HardDelete(TEntity entity) => base.Delete(entity);

    public void Undelete(TEntity entity)
    {
        entity.IsDeleted = false;
        entity.DeletedOn = null;
        this.Update(entity);
    }

    public override void Delete(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.DeletedOn = DateTime.UtcNow;
        this.Update(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            this.Delete(entity);
        }
    }

    public void HardDeleteRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            this.HardDelete(entity);
        }
    }
}