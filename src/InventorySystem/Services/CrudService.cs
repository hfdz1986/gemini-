using System.Linq.Expressions;
using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Services;

/// <summary>
/// 通用 CRUD 服务基类。新增一个主数据模块时，只需继承它即可获得
/// 列表 / 单条 / 新增 / 修改 / 删除 的标准实现，这是本框架可复用的核心。
/// 使用 IDbContextFactory 为每次操作创建独立的 DbContext，符合 Blazor Server 的最佳实践。
/// </summary>
public class CrudService<T> where T : BaseEntity
{
    protected readonly IDbContextFactory<AppDbContext> Factory;

    public CrudService(IDbContextFactory<AppDbContext> factory) => Factory = factory;

    /// <summary>子类可重写以预加载导航属性。</summary>
    protected virtual IQueryable<T> Query(AppDbContext db) => db.Set<T>();

    public virtual async Task<List<T>> GetAllAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await Query(db).AsNoTracking().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await Query(db).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await Query(db).AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual async Task<int> CountAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        return await db.Set<T>().CountAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await using var db = await Factory.CreateDbContextAsync();
        db.Set<T>().Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await using var db = await Factory.CreateDbContextAsync();
        db.Set<T>().Update(entity);
        // 保留原始创建时间，避免被表单回传的默认值覆盖
        db.Entry(entity).Property(e => e.CreatedAt).IsModified = false;
        await db.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        await using var db = await Factory.CreateDbContextAsync();
        var entity = await db.Set<T>().FindAsync(id);
        if (entity is not null)
        {
            db.Set<T>().Remove(entity);
            await db.SaveChangesAsync();
        }
    }

    /// <summary>保存（Id==0 视为新增，否则更新）。</summary>
    public virtual async Task SaveAsync(T entity)
    {
        if (entity.Id == 0)
            await AddAsync(entity);
        else
            await UpdateAsync(entity);
    }
}
