using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Scaffolding.Core.Identity;

namespace Scaffolding.Web.Utilities.Middleware.Authorization
{
    public class OrgDivDataHandler : AuthorizationHandler<OrgDivDataRequirement>
    {
	    private readonly ILogger _logger;
	    private readonly IRequestReader _reader;

	    public OrgDivDataHandler(ILoggerFactory loggerFactory, IRequestReader reader)
	    {
		    _logger = loggerFactory.CreateLogger(this.GetType().FullName);
		    _reader = reader;
	    }

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrgDivDataRequirement requirement)
		{
			_logger.LogDebug($"Attempting to access resource protected with the {requirement.RequirementName} handler.");

			DataIds ids = _reader.GetDataIds(context.Resource);
			bool canAccess = CheckData(context.User, requirement.RequiredGroups, ids);

			if (canAccess)
			{
				_logger.LogDebug($"Principal WAS ABLE access resource protected with the {requirement.RequirementName} handler.  Principal Summary: {context.User.ForLogging()}");
				context.Succeed(requirement);
			}
			else
			{
				_logger.LogDebug($"Principal IS UNABLE to access resource protected with the {requirement.RequirementName} handler.  Principal Summary: {context.User.ForLogging()}");
				context.Fail();
			}

			return Task.CompletedTask;
		}

		public bool CheckData(ClaimsPrincipal principal, List<string> requiredGroups, DataIds ids)
		{
			// TODO: when we tie console tokens to users this will have to change
			if (principal.IsAdmin() || !principal.IsUserIdentiy())
			{
				return true;
			}

			if (ids == null)
			{
				return false;
			}

			if (ids.OrgNumber > 0 && principal.IsOrgAdmin(ids.OrgNumber))
			{
				return true;
			}

			for (int i = 0; i < requiredGroups.Count; i++)
			{
				bool canAccess = false;

				if (ids.OrgNumber > 0 && ids.DivNumber > 0)
				{
					canAccess = principal.HasGroupWithOrg(requiredGroups[i], ids.OrgNumber) &&
					            principal.HasGroupWithDivision(requiredGroups[i], ids.DivNumber);
				}

				if (ids.OrgNumber > 0 && ids.DivNumber == 0)
				{
					canAccess = principal.HasGroupWithOrg(requiredGroups[i], ids.OrgNumber);
				}

				if (ids.OrgNumber == 0 && ids.DivNumber > 0)
				{
					canAccess = principal.HasGroupWithDivision(requiredGroups[i], ids.DivNumber);
				}

				if (canAccess)
				{
					return true;
				}
			}

			return false;
		}
	}
}
