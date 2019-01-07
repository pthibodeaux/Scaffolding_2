namespace Scaffolding.Core.Tests.Fixtures
{
	public interface ISomeManager { }

	public class SomeManager : ISomeManager { }

	public class ConsumesManager
    {
	    public ISomeManager Manager { get; set; }

	    public ConsumesManager(ISomeManager manager)
	    {
		    Manager = manager;
	    }
    }
}
