using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
    public static class BaseRepositoryExtensions
    {
	    public static IUnitOfWork CreateUnitOfWork(this IEnumerable<IBaseRepository> repos)
	    {
		    if (repos.Any())
		    {
			    return repos.First().CreateUnitOfWork(repos);
		    }

		    throw new NullReferenceException("Attempted to create an IUnitOfWork from either a null or empty list of IBaseRepositories.");
	    }

	    public static async Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork(this IEnumerable<IBaseRepository> repos)
	    {
			if (repos.Any())
			{
				return await repos.First().CreateAsyncUnitOfWork(repos);
		    }

		    throw new NullReferenceException("Attempted to create an IAsyncUnitOfWork from either a null or empty list of IBaseRepositories.");
		}
    }
}
