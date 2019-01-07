using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Scaffolding.Core.Identity;
using Scaffolding.Web.Utilities.Middleware.Authorization;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Middleware.Authorization
{
    public class OrgDivDataHandlerTests : BaseAuthHandlerTest
    {
		private readonly OrgDivDataHandler _handler;
	    private readonly Mock<IRequestReader> _reader;

	    public OrgDivDataHandlerTests()
	    {
		    _reader = new Mock<IRequestReader>();
		    _handler = new OrgDivDataHandler(CreateMockLogger(), _reader.Object);
		}

	    private AuthorizationFilterContext SetupResource()
	    {
		    DefaultHttpContext httpCtx = new DefaultHttpContext();

		    ActionContext actionCtx = new ActionContext(httpCtx, new RouteData(), new ActionDescriptor());
			return new AuthorizationFilterContext(actionCtx, new List<IFilterMetadata>());
	    }

		[Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_User_Is_Admin()
	    {
		    SetUpPrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.APP_ADMIN, "True")
			});
		    SetUpAuthContext(OrgDivDataRequirement.DivWatchRequirement);

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_User_Is_Console()
	    {
		    SetUpPrincipal(new List<Claim>{ }, "name", "something");
		    SetUpAuthContext(OrgDivDataRequirement.DivContribRequirement);

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_Client_Is_UnAuthorized_Swagger()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim("name", "their name"),
				new Claim(SecurityConstants.CLIENT_CLAIM, SecurityConstants.SWAGGER_CLIENT)
		    }, "name", SecurityConstants.IDENTITY_HUB_USER_AUTH_TYPE);
		    SetUpAuthContext(OrgDivDataRequirement.DivContribRequirement);

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
	    }

		[Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Not_Admin_No_Ids()
	    {
		    SetUpPrincipal(new List<Claim> { });
		    SetUpAuthContext(OrgDivDataRequirement.OrgAdminRequirement);

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_User_Org_Admin_With_Header()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:5")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.OrgAdminRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "5"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Org_Admin_With_Wrong_Id()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:5")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.OrgAdminRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "10"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Div_Contrib_With_Wrong_Group()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.DIV_WATCH, $"{SecurityConstants.ORG_ID}:5"),
			    new Claim(SecurityConstants.DIV_WATCH, $"{SecurityConstants.DIV_ID}:10")
			});
		    SetUpAuthContext(OrgDivDataRequirement.DivContribRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "5", DivisionId = "10"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Div_Contrib_With_Wrong_Org()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.ORG_ID}:5"),
			    new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:10")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.DivContribRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "15", DivisionId = "10"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Div_Contrib_With_Wrong_Div()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.ORG_ID}:5"),
			    new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:10")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.DivContribRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "5", DivisionId = "15"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_User_Div_Contrib_With_Correct_Data()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.ORG_ID}:5"),
			    new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:10")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.DivContribRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "5", DivisionId = "10"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
		}

	    [Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_User_Div_Admin_With_Correct_Data()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.DIV_ADMIN, $"{SecurityConstants.ORG_ID}:5"),
			    new Claim(SecurityConstants.DIV_ADMIN, $"{SecurityConstants.DIV_ID}:10")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.DivWatchRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "5", DivisionId = "10"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
	    }

	    [Fact]
	    public async void HandleRequirementAsync_Should_Succeed_When_User_Cage_User_With_Correct_Data()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.CAGE_USER, $"{SecurityConstants.ORG_ID}:5"),
			    new Claim(SecurityConstants.CAGE_USER, $"{SecurityConstants.DIV_ID}:10")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.CageRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "5", DivisionId = "10"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeTrue();
		    _authContext.HasFailed.Should().BeFalse();
	    }

	    [Fact]
	    public async void HandleRequirementAsync_Should_Fail_When_User_Cage_User_With_Incorrect_Data()
	    {
		    SetUpPrincipal(new List<Claim>
		    {
			    new Claim(SecurityConstants.CAGE_USER, $"{SecurityConstants.ORG_ID}:5"),
			    new Claim(SecurityConstants.CAGE_USER, $"{SecurityConstants.DIV_ID}:15")
		    });
		    SetUpAuthContext(OrgDivDataRequirement.CageRequirement, SetupResource());
		    _reader.Setup(r => r.GetDataIds(It.IsAny<object>())).Returns(new DataIds {OrganizationId = "5", DivisionId = "10"});

		    await _handler.HandleAsync(_authContext);

		    _authContext.HasSucceeded.Should().BeFalse();
		    _authContext.HasFailed.Should().BeTrue();
	    }
	}
}
