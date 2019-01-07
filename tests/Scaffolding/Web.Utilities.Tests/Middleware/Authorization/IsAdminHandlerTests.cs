using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Scaffolding.Core.Identity;
using Scaffolding.Web.Utilities.Middleware.Authorization;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Middleware.Authorization
{
    public class IsAdminHandlerTests : BaseAuthHandlerTest
    {
		private readonly IsAdminHandler _handler;

	    public IsAdminHandlerTests()
	    {
		    _handler = new IsAdminHandler(CreateMockLogger());
		}

		[Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_User_Is_Admin()
	    {
		    SetUpPrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.APP_ADMIN, "True"),
				new Claim("name", "the name")
			}, "name");
		    SetUpAuthContext(new AdminRequirement());

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_Prinipal_Not_From_CRM()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.CLIENT_CLAIM, SecurityConstants.CONSOLE_CLIENT)
		    }, "name", "something");
		    SetUpAuthContext(new AdminRequirement());

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
	    }

		[Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Not_Admin_And_From_CRM()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim("name", "the name"),
			    new Claim(SecurityConstants.CLIENT_CLAIM, SecurityConstants.CRM_CLIENT)
			}, "name");
			SetUpAuthContext(new AdminRequirement());

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Not_Admin_And_No_Client_Specified()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim("name", "the name")
		    }, "name");
		    SetUpAuthContext(new AdminRequirement());

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
	    }
	}
}
