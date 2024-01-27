using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class BaseRepository<TModel, T>(DbContext context) : IBaseRepository<TModel, T> where TModel : BaseModel<T>
{
    protected readonly DbContext Context = context;
    protected readonly DbSet<TModel> Entities = context.Set<TModel>();

    public async Task<List<TModel>> GetAll() => await Entities.ToListAsync();

    public IQueryable<TModel> GetAllQuery() => Entities;

    public async Task<TModel?> Get(T id) => await Entities.FirstOrDefaultAsync(m => Equals(m.Id, id));

    public async Task<TModel> Create(TModel model)
    {
        await Entities.AddAsync(model);
        await Context.SaveChangesAsync();

        return model;
    }

    public async Task<bool> Delete(T id)
    {
        TModel? toDelete = await Entities.FirstOrDefaultAsync(m => Equals(m.Id, id));

        if (toDelete is not null)
        {
            Entities.Remove(toDelete);
            await Context.SaveChangesAsync();

            return true;
        }

        return false;
    }
}