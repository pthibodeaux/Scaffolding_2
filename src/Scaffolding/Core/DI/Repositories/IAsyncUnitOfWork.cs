using System.Threading.Tasks;

namespace Scaffolding.Core.DI.Repositories
{
    public interface IAsyncUnitOfWork : IBaseUnitOfWork
    {
	    Task BeginTransactionAsync();
    }
}
