using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using DapperExtensions;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public class AsyncUnitOfWork : BaseUnitOfWork, IAsyncConnection
	{
	    public AsyncUnitOfWork(IDbConnectionProvider provider, IEnumerable<IBaseRepository> repos) : base(provider, repos)
	    {
		    Repositories.ForEach(r => r.AsyncUnitOfWork = this);
	    }

	    public async Task OpenAsync()
	    {
		    if (DbConnection == null)
		    {
			    DbConnection = Provider.GetConnection();
		    }
		    
		    if (!IsOpen)
		    {
			    await DbConnection.OpenAsync();
		    }
	    }

		public async Task BeginTransactionAsync()
		{	
			// transaction needs to be created BEFORE connection
			if (TransactionScope == null)
			{
				TransactionScope = CreateTransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			}

			// connection needs to be created AFTER transaction
			await OpenAsync();
		    
			if (DbTransaction == null)
			{
				DbTransaction = DbConnection.BeginTransaction();
			}
		}

	    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object arg = null)
		{
			return await DbConnection.QueryAsync<T>(new CommandDefinition(sql, arg, DbTransaction));
	    }

	    public async Task<T> QuerySingleAsync<T>(string sql, object arg = null)
	    {
			// Dapper does not currently support passing  in a CancellationToken for QuerySingleOrDefaultAsync - might come out in version 2.0
			return await DbConnection.QuerySingleOrDefaultAsync<T>(sql, arg, DbTransaction);
		}

		public async Task<int> ExecuteAsync(string sql, object arg = null, CommandType? commandType = null)
	    {
		    return await DbConnection.ExecuteAsync(sql, arg, DbTransaction, commandType: commandType);
	    }

	    public async Task<T> GetAsync<T>(object id) where T : class
	    {
			// Dapper does not currently support passing  in a CancellationToken for GetAsync - might come out in version 2.0
			return await DbConnection.GetAsync<T>(id, DbTransaction);
		}

		public async Task<object> InsertAsync<T>(T obj) where T : class
	    {
		    return await DbConnection.InsertAsync(obj, DbTransaction);
	    }

	    public async Task<bool> UpdateAsync<T>(T obj) where T : class
	    {
		    return await DbConnection.UpdateAsync(obj, DbTransaction);
	    }

	    public async Task<bool> DeleteAsync<T>(T obj) where T : class
	    {
		    return await DbConnection.DeleteAsync(obj, DbTransaction);
		}

	    public async Task UseConnectionAsync(Func<IDbConnection, IDbTransaction, Task> work)
	    {
		    await work(DbConnection, DbTransaction);
	    }

		public override void CommitTransaction()
	    {
		    base.CommitTransaction();
		    Repositories.ForEach(r => r.AsyncUnitOfWork = null);
	    }

	    public override void RollbackTransaction()
	    {
		    base.RollbackTransaction();
		    Repositories.ForEach(r => r.AsyncUnitOfWork = null);
	    }

	    public override void Dispose()
	    {
		    base.Dispose();
		    Repositories.ForEach(r => r.AsyncUnitOfWork = null);
	    }
	}
}
