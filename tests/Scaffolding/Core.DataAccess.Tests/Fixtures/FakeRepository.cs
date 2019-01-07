using System.Collections.Concurrent;

namespace Scaffolding.Core.DataAccess.Tests.Fixtures
{
    public class FakeRepository : BaseRepository
    {
	    public FakeRepository(IUnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider) { }

	    public ConcurrentDictionary<string, string> Statements => _statements;
    }
}
