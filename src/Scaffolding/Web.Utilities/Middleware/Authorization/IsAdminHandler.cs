using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Scaffolding.Core.Identity;

namespace Scaffolding.Web.Utilities.Middleware.Authorization
{
    public class IsAdminHandler : AuthorizationHandler<AdminRequirement>
	{
		private readonly ILogger _logger;

		public IsAdminHandler(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger(this.GetType().FullName);
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
		{
			// TODO: when we tie console tokens to users this will have to change
			if (context.User.IsAdmin() || context.User.ComesFromClient(SecurityConstants.CONSOLE_CLIENT))
			{
				_logger.LogDebug($"Principal WAS ABLE access resource protected with the Admin handler.  Principal Summary: {context.User.ForLogging()}");
				context.Succeed(requirement);
			}
			else
			{
				_logger.LogDebug($"Principal IS UNABLE to access resource protected with the Admin handler.  Principal Summary: {context.User.ForLogging()}");
				context.Fail();
			}

			return Task.CompletedTask;
		}
	}
}
