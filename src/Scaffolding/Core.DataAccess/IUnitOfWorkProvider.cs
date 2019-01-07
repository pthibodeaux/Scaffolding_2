using System.Collections.Generic;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public interface IUnitOfWorkProvider
    {
	    IConnection GetUnitOfWork(IEnumerable<IBaseRepository> repositories);
		IAsyncConnection GetAsyncUnitOfWork(IEnumerable<IBaseRepository> repositories);
	}
}
