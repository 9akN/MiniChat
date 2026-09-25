using System.Linq.Expressions;

namespace ChatApp.Api.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
	Task<T?> GetByIdAsync(int id);
	Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
	IQueryable<T> Query();
	Task AddAsync(T entity);
	void Remove(T entity);
}
