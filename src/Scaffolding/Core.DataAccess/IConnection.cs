using System;
using System.Collections.Generic;
using System.Data;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public interface IConnection : IUnitOfWork
	{
		void Open();
		List<T> Query<T>(string sql, object arg = null);
	    T QuerySingle<T>(string sql, object arg = null);
	    int Execute(string sql, object arg = null);
	    T Get<T>(object id) where T : class;
	    object Insert<T>(T obj) where T : class;
	    bool Update<T>(T obj) where T : class;
		bool Delete<T>(T obj) where T : class;
		void UseConnection(Action<IDbConnection, IDbTransaction> work);
		int NumberOfRepos { get; }

	}
}