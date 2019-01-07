using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scaffolding.Core.DataAccess;
using Scaffolding.Core.DI.Repositories;

namespace Common.Test.Utilities
{
    public interface IBaseRepositoryTest { }

	public static class IBaseRepositoryTestExtentions
	{
		public static void WrapTestDbOperations(this IBaseRepositoryTest tests, List<IBaseRepository> repos, Action work)
		{
			using (IUnitOfWork uow = repos.CreateUnitOfWork())
			{
				try
				{
					uow.BeginTransaction();
					work();
				}
				finally
				{
					uow.RollbackTransaction();
				}
			}
		}
		public static async Task WrapTestDbOperationsAsync(this IBaseRepositoryTest tests, List<IBaseRepository> repos, Func<Task> work)
		{
			using (IAsyncUnitOfWork uow = await repos.CreateAsyncUnitOfWork())
			{
				try
				{
					await uow.BeginTransactionAsync();
					await work();
				}
				finally
				{
					uow.RollbackTransaction();
				}
			}
		}
	}
}
