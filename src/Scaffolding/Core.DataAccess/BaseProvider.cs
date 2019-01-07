using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Options;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public abstract class BaseProvider<T> : IUnitOfWorkProvider, IDbConnectionProvider where T : class, new()
    {
	    protected T _config;

	    protected BaseProvider() { }

	    protected BaseProvider(IOptions<T> config) : this(config.Value) { }

	    protected BaseProvider(T config)
	    {
			_config = config;
		}

	    public K Populate<K>(T config) where K : class, IUnitOfWorkProvider
		{
		    _config = config;
			return this as K;
		}

	    public virtual IConnection GetUnitOfWork(IEnumerable<IBaseRepository> repositories)
	    {
		    return new UnitOfWork(this, repositories);
	    }

	    public virtual IAsyncConnection GetAsyncUnitOfWork(IEnumerable<IBaseRepository> repositories)
	    {
		    return new AsyncUnitOfWork(this, repositories);
	    }

	    public abstract IDbConnection GetConnection();
    }
}
