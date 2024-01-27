using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class BaseRepository<TModel, T>(DbContext context) : IBaseRepository<TModel, T> where TModel : BaseModel<T>
{
    protected readonly DbContext Context = context;
    protected readonly DbSet<TModel> Entities = context.Set<TModel>();

    public IQueryable<TModel> GetAllQuery() => Entities;

    public async Task<TModel> Create(TModel model)
    {
        await Entities.AddAsync(model);
        await Context.SaveChangesAsync();

        return model;
    }
}