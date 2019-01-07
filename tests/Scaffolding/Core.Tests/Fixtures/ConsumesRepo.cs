using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.Tests.Fixtures
{
	public interface ISomeRepo : IBaseRepository { }
	
	public class ConsumesRepo
    {
	    public ISomeRepo Repository { get; set; }

	    public ConsumesRepo(ISomeRepo repo)
	    {
		    Repository = repo;
	    }
	}
}
