using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using Scaffolding.Core.Identity;

namespace Scaffolding.Web.Utilities.Tests.Middleware.Authorization
{
    public abstract class BaseAuthHandlerTest
	{
		protected ClaimsIdentity _identity;
		protected ClaimsPrincipal _principal;
		protected AuthorizationHandlerContext _authContext;

		protected BaseAuthHandlerTest()
		{
			_identity = null;
			_principal = null;
			_authContext = null;
		}

		protected ILoggerFactory CreateMockLogger()
		{
			Mock<ILogger> logger = new Mock<ILogger>();
			logger.SetupAllProperties();

			Mock<ILoggerFactory> factory = new Mock<ILoggerFactory>();
			factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

			return factory.Object;
		}

		protected void SetUpPrincipal(IEnumerable<Claim> claims, string nameClaim = null, string authType = null)
		{
			_identity = new ClaimsIdentity(claims, authType ?? SecurityConstants.IDENTITY_HUB_USER_AUTH_TYPE, nameClaim, null);
			_principal = new ClaimsPrincipal(_identity);
		}

		protected void SetUpAuthContext(IAuthorizationRequirement requirement, object resource = null)
		{
			_authContext = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, _principal, resource ?? new object());
		}

	}
}
