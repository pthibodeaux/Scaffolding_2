using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.Tests.Fixtures
{
	public class SomeRepo : ISomeRepo
	{

		public ConcurrentDictionary<string, string> Statements = new ConcurrentDictionary<string, string>();

	    public IUnitOfWork UnitOfWork { get; set; }
	    public IAsyncUnitOfWork AsyncUnitOfWork { get; set; }

	    public IUnitOfWork CreateUnitOfWork(IEnumerable<IBaseRepository> repos)
	    {
		    throw new NotImplementedException();
	    }

		public IUnitOfWork CreateUnitOfWork(IBaseRepository repo)
		{
			throw new NotImplementedException();
		}

		public IUnitOfWork CreateUnitOfWork()
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork(IEnumerable<IBaseRepository> repos)
	    {
		    throw new NotImplementedException();
	    }

		public Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork(IBaseRepository repo)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork()
		{
			throw new NotImplementedException();
		}

		public string GetSql(string resource)
	    {
		    throw new NotImplementedException();
	    }

		public string GetSqlFromResource(string resource)
		{
			throw new NotImplementedException();
		}

		public List<T> Query<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<T>> QueryAsync<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public T QuerySingle<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public Task<T> QuerySingleAsync<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public int Execute(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public Task<int> ExecuteAsync(string sql, object arg = null, CommandType? commandType = null)
		{
			throw new NotImplementedException();
		}
		
		public T Get<T>(object id) where T : class
		{
			throw new NotImplementedException();
		}

		public async Task<T> GetAsync<T>(object id) where T : class
		{
			throw new NotImplementedException();
		}

		public object Insert<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public Task<object> InsertAsync<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public bool Update<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public async Task<bool> UpdateAsync<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public bool Delete<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public async Task<bool> DeleteAsync<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}
	}
}