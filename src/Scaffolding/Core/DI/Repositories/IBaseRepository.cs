using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Scaffolding.Core.DI.Repositories
{
	public interface IBaseRepository
	{
		IUnitOfWork CreateUnitOfWork(IEnumerable<IBaseRepository> repos);
		IUnitOfWork CreateUnitOfWork(IBaseRepository repo);
		IUnitOfWork CreateUnitOfWork();
		Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork(IEnumerable<IBaseRepository> repos);
		Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork(IBaseRepository repo);
		Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork();
		List<T> Query<T>(string sql, object arg = null);
		Task<IEnumerable<T>> QueryAsync<T>(string sql, object arg = null);
		T QuerySingle<T>(string sql, object arg = null);
		Task<T> QuerySingleAsync<T>(string sql, object arg = null);
		int Execute(string sql, object arg = null);
		Task<int> ExecuteAsync(string sql, object arg = null, CommandType? commandType = null);
		T Get<T>(object id) where T : class;
		Task<T> GetAsync<T>(object id) where T : class;
		object Insert<T>(T obj) where T : class;
		Task<object> InsertAsync<T>(T obj) where T : class;
		bool Update<T>(T obj) where T : class;
		Task<bool> UpdateAsync<T>(T obj) where T : class;
		bool Delete<T>(T obj) where T : class;
		Task<bool> DeleteAsync<T>(T obj) where T : class;
	}
}