using System.Linq.Expressions;
using ChatApp.Api.Data;
using ChatApp.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
	private readonly DbSet<T> _set = context.Set<T>();

	public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);

	public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
		await _set.Where(predicate).ToListAsync();

	public IQueryable<T> Query() => _set.AsQueryable();

	public async Task AddAsync(T entity) => await _set.AddAsync(entity);

	public void Remove(T entity) => _set.Remove(entity);
}
