namespace DAL.Repositories;

public interface IBaseRepository<TModel, T> where TModel : BaseModel<T>
{
    IQueryable<TModel> GetAllQuery();
    Task<TModel> Create(TModel model);
}