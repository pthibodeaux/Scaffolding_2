using System;

namespace Scaffolding.Core.DI.Repositories
{
    public interface IBaseUnitOfWork : IDisposable
    {
	    bool IsOpen { get; }
	    bool InTransaction { get; }
	    bool Disposed { get; }
	    void CommitTransaction();
	    void RollbackTransaction();
    }
}
