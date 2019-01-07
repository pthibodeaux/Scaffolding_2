using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public interface IAsyncConnection : IAsyncUnitOfWork
    {
	    Task OpenAsync();
		Task<IEnumerable<T>> QueryAsync<T>(string sql, object arg = null);
	    Task<T> QuerySingleAsync<T>(string sql, object arg = null);
	    Task<int> ExecuteAsync(string sql, object arg = null, CommandType? commandType = null);
	    Task<T> GetAsync<T>(object id) where T : class;
	    Task<object> InsertAsync<T>(T obj) where T : class;
		Task<bool> UpdateAsync<T>(T obj) where T : class;
	    Task<bool> DeleteAsync<T>(T obj) where T : class;
	    Task UseConnectionAsync(Func<IDbConnection, IDbTransaction, Task> work);
	    int NumberOfRepos { get; }

	}
}
