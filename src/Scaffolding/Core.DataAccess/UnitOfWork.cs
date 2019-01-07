using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using Dapper;
using DapperExtensions;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public class UnitOfWork : BaseUnitOfWork, IConnection
	{
	    public UnitOfWork(IDbConnectionProvider provider, IEnumerable<IBaseRepository> repos = null) : base(provider, repos)
	    {
		    Repositories.ForEach(r => r.UnitOfWork = this);
		}
		
		public void Open()
		{
			if (DbConnection == null)
			{
				DbConnection = Provider.GetConnection();
			}
			
			if (!IsOpen)
			{
				DbConnection.Open();
			}
		}

		public void BeginTransaction()
		{
			// transaction needs to be created BEFORE connection
			if (TransactionScope == null)
			{
				TransactionScope = CreateTransactionScope(TransactionScopeAsyncFlowOption.Suppress);
			}

			// connection needs to be created AFTER transaction
			Open();
		    
			if (DbTransaction == null)
			{
				DbTransaction = DbConnection.BeginTransaction();
			}
		}

	    public List<T> Query<T>(string sql, object arg = null)
	    {
		    return new List<T>(DbConnection.Query<T>(sql, arg, DbTransaction) ?? new T[0]);
	    }

	    public T QuerySingle<T>(string sql, object arg = null)
	    {
		    return DbConnection.QuerySingleOrDefault<T>(sql, arg, DbTransaction);

	    }

		public int Execute(string sql, object arg = null)
	    {
		    return DbConnection.Execute(sql, arg, DbTransaction);
		}

	    public T Get<T>(object id) where T : class
		{
		    return DbConnection.Get<T>(id, DbTransaction);
	    }

		public object Insert<T>(T obj) where T : class
		{
			return DbConnection.Insert(obj, DbTransaction);
		}

	    public bool Update<T>(T obj) where T : class
		{
		    return DbConnection.Update(obj, DbTransaction);
		}

	    public bool Delete<T>(T obj) where T : class
		{
			return DbConnection.Delete(obj, DbTransaction);
		}

		public void UseConnection(Action<IDbConnection, IDbTransaction> work)
		{
			work(DbConnection, DbTransaction);
		}

		public override void CommitTransaction()
		{
			base.CommitTransaction();
			Repositories.ForEach(r => r.UnitOfWork = null);
		}

		public override void RollbackTransaction()
		{
			base.RollbackTransaction();
			Repositories.ForEach(r => r.UnitOfWork = null);
		}

		public override void Dispose()
		{
			base.Dispose();
			Repositories.ForEach(r => r.UnitOfWork = null);
		}
	}
}
