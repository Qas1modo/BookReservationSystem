using DAL.Models;
using System.Linq.Expressions;

namespace Infrastructure.Query
{
	public interface IQuery<TEntity> where TEntity : BaseEntity, new()
	{
		IQuery<TEntity> Where<T>(Expression<Func<T, bool>> rootPredicate, string columnName);

		IQuery<TEntity> OrderBy<T>(string columnName, bool ascending = true) where T : IComparable<T>;

		IQuery<TEntity> Page(int page, int pageSize);
		Task<QueryResult<TEntity>> Execute();
	}
}
