namespace DAL.Repositories;

public interface IBaseRepository<TModel, T> where TModel : BaseModel<T>
{
    Task<List<TModel>> GetAll();
    IQueryable<TModel> GetAllQuery();
    Task<TModel?> Get(T id);
    Task<TModel> Create(TModel model);
    Task<bool> Delete(T id);
}