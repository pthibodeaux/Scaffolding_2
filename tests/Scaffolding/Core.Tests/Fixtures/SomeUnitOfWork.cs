using System;
using System.Collections.Generic;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.Tests.Fixtures
{
	public class SomeUnitOfWork : IUnitOfWork
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

		public List<T> Query<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public T QuerySingle<T>(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public int Execute(string sql, object arg = null)
		{
			throw new NotImplementedException();
		}

		public T Get<T>(object id) where T : class
		{
			throw new NotImplementedException();
		}

		public object Insert<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public bool Update<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}

		public bool Delete<T>(T obj) where T : class
		{
			throw new NotImplementedException();
		}
	}
}