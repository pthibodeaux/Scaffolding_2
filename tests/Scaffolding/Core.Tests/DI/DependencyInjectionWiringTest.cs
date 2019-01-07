using FluentAssertions;
using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffolding.Core.DI;
using Scaffolding.Core.Tests.Fixtures;
using Xunit;

namespace Scaffolding.Core.Tests.DI
{
    public class DependencyInjectionWiringTest
    {
	    private readonly IServiceCollection _services;
	    private readonly IConfigurationRoot _config;

		public DependencyInjectionWiringTest()
		{
			_services = new DummyServiceCollection();
		    _config = DependencyInjectionWiring.BuildConfig("Local");
	    }

	    [Fact]
	    public void GetInstance_Initialization_When_Core_Matches_Throw_OK_When_Repo_Requested()
	    {
		    Container c = new Container(sr =>
		    {
				sr.InitializeIOC();
		    });
		    ConsumesRepo repo = c.GetInstance<ConsumesRepo>();

		    repo.Should().NotBeNull();
	    }

		[Fact]
	    public void GetInstance_When_Default_Initialization_OK_When_Service_Requested()
	    {
		    Container c = new Container(sr =>
		    {
			    sr.InitializeIOC();
		    });
		    ConsumesManager service = c.GetInstance<ConsumesManager>();

		    service.Manager.Should().NotBeNull();
	    }
	}
}
