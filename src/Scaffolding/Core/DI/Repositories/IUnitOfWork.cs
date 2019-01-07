namespace Scaffolding.Core.DI.Repositories
{
    public interface IUnitOfWork : IBaseUnitOfWork
	{
		void BeginTransaction();
    }
}