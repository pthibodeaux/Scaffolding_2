using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.Tests.Fixtures
{
	public class SomeAsyncUnitOfWork : IAsyncUnitOfWork
	{
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public bool IsOpen { get; }
		public bool InTransaction { get; }
		public bool Disposed { get; }
		public void BeginTransaction()
		{
			throw new NotImplementedException();
		}

		public void CommitTransaction()
		{
			throw new NotImplementedException();
		}

		public void RollbackTransaction()
		{
			throw new NotImplementedException();
		}

		public Task BeginTransactionAsync()
		{
			throw new NotImplementedException();
		}

		public Task Open()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<T>> QueryAsync<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public Task<T> QuerySingleAsync<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public Task<int> ExecuteAsync(string sql, object arg = null, CommandType? commandType = null)
		{
			throw new NotImplementedException();
		}

		public Task<T> GetAsync<T>(object id) where T : class
		{
			throw new NotImplementedException();
		}

		public Task<object> InsertAsync<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateAsync<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteAsync<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}
	}
}
