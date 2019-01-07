using System.Collections.Generic;
using System.Data;
using System.Transactions;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public abstract class BaseUnitOfWork : IBaseUnitOfWork
    {
	    protected readonly IDbConnectionProvider Provider;
	    
	    protected BaseUnitOfWork(IDbConnectionProvider provider, IEnumerable<IBaseRepository> repos)
		{
			Provider = provider;

			if (repos != null)
			{
				foreach (IBaseRepository repo in repos)
				{
					if (repo is IConnectionRepo connectionRepo)
					{
						Repositories.Add(connectionRepo);
					}
				}
			}
		}
		
	    public bool Disposed { get; private set; } = false;
	    protected IDbConnection DbConnection { get; set; }
	    protected IDbTransaction DbTransaction { get; set; }
	    protected TransactionScope TransactionScope { get; set; }
	    protected List<IConnectionRepo> Repositories { get; } = new List<IConnectionRepo>();
		
	    public bool IsOpen => DbConnection?.State != ConnectionState.Closed && DbConnection?.State != ConnectionState.Broken;
	    public bool InTransaction => DbTransaction != null;

	    protected TransactionScope CreateTransactionScope(TransactionScopeAsyncFlowOption asyncFlow)
	    {
		    return new TransactionScope(TransactionScopeOption.RequiresNew, asyncFlow);
	    }

	    public virtual void CommitTransaction()
	    {
		    DbTransaction?.Commit();
		    TransactionScope?.Complete();
		    Dispose();
	    }

	    public virtual void RollbackTransaction()
	    {
		    DbTransaction?.Rollback();
		    Dispose();
	    }
		
	    public virtual void Dispose()
	    {
		    DbTransaction?.Dispose();
		    DbConnection?.Dispose();
		    TransactionScope?.Dispose();
		    DbTransaction = null;
		    DbConnection = null;
		    TransactionScope = null;
		    Transaction.Current = null;
		    Disposed = true;
		}

	    public int NumberOfRepos => Repositories.Count;
    }
}
