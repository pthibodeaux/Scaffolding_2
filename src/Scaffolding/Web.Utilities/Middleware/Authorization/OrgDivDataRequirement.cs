using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Scaffolding.Core.Identity;

namespace Scaffolding.Web.Utilities.Middleware.Authorization
{
	public class OrgDivDataRequirement : IAuthorizationRequirement
	{
		public OrgDivDataRequirement() { }

		public OrgDivDataRequirement(IEnumerable<string> groups)
		{
			RequiredGroups.AddRange(groups ?? new string[0]);
		}

        public List<string> RequiredGroups { get; } = new List<string>();

		public string RequirementName => RequiredGroups.Last();

        public static OrgDivDataRequirement OrgAdminRequirement = new OrgDivDataRequirement(new [] { SecurityConstants.ORG_ADMIN });
		public static OrgDivDataRequirement DivAdminRequirement = new OrgDivDataRequirement(new[] { SecurityConstants.ORG_ADMIN, SecurityConstants.DIV_ADMIN });
		public static OrgDivDataRequirement DivContribRequirement = new OrgDivDataRequirement(new[]
		{
			SecurityConstants.ORG_ADMIN,
			SecurityConstants.DIV_ADMIN,
			SecurityConstants.DIV_CONTRIB
		});
		public static OrgDivDataRequirement DivWatchRequirement = new OrgDivDataRequirement(new[]
		{
			SecurityConstants.ORG_ADMIN,
			SecurityConstants.DIV_ADMIN,
			SecurityConstants.DIV_CONTRIB,
			SecurityConstants.DIV_WATCH
		});
        public static OrgDivDataRequirement CageRequirement = new OrgDivDataRequirement(new []
        {
            SecurityConstants.CAGE_USER
        });
	}
}
