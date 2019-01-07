using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Scaffolding.Core.Identity
{
	public static class ClaimsPrincipalExtentions
	{
		public static bool IsAdmin(this ClaimsPrincipal principal)
		{
			return principal.HasGroup(SecurityConstants.APP_ADMIN);
		}

		public static bool IsOrgAdmin(this ClaimsPrincipal principal, int orgId)
		{
			return principal.HasGroupWithOrg(SecurityConstants.ORG_ADMIN, orgId);
		}

	    public static int GetUserId(this ClaimsPrincipal principal)
	    {
	        string claimVal = "";
	        try
	        {
	            claimVal = principal.Claims.First(c => c.Type == "id")?.Value;
            }
	        catch
	        {
	            throw new Exception("No ID in Claim!");
            }

	        if (string.IsNullOrWhiteSpace(claimVal))
	        {
                throw new Exception("Invalid user ID!");
	        }

	        return int.Parse(claimVal);
	    }

		public static bool HasGroup(this ClaimsPrincipal principal, string group)
		{
			return principal.HasClaim(c => c.Type == group);
		}

		public static bool HasGroupWithOrg(this ClaimsPrincipal principal, string group, int orgId)
		{
			return principal.FindClaim(group, orgId, SecurityConstants.ORG_ID);
		}

		public static bool HasGroupWithDivision(this ClaimsPrincipal principal, string group, int divId)
		{
			return principal.FindClaim(group, divId, SecurityConstants.DIV_ID);
		}

		public static bool IsUserIdentiy(this ClaimsPrincipal principal)
		{
			return !string.IsNullOrWhiteSpace(principal.Identity.Name) ||
			       principal.Identity.AuthenticationType == SecurityConstants.IDENTITY_HUB_USER_AUTH_TYPE;
		}

		public static string ForLogging(this ClaimsPrincipal principal)
		{
			StringBuilder builder = new StringBuilder($"Name : {principal.Identity.Name}");

			builder.AppendLine($"AuthType : {principal.Identity.AuthenticationType}");
			builder.AppendLine($"Email : {principal.FindFirst(c => c.Type == "email")?.Value}");
			builder.AppendLine($"IsAdmin : {principal.IsAdmin()}");
			builder.AppendLine($"HasGroup({SecurityConstants.APP_ADMIN}) : {principal.HasGroup(SecurityConstants.APP_ADMIN)}");
			builder.AppendLine($"HasGroup({SecurityConstants.ORG_ADMIN}) : {principal.HasGroup(SecurityConstants.ORG_ADMIN)}");
			builder.AppendLine($"HasGroup({SecurityConstants.DIV_ADMIN}) : {principal.HasGroup(SecurityConstants.DIV_ADMIN)}");
			builder.AppendLine($"HasGroup({SecurityConstants.DIV_CONTRIB}) : {principal.HasGroup(SecurityConstants.DIV_CONTRIB)}");
			builder.AppendLine($"HasGroup({SecurityConstants.DIV_WATCH}) : {principal.HasGroup(SecurityConstants.DIV_WATCH)}");

			foreach (Claim claim in principal.FindAll(c => SecurityConstants.AllGroups.Contains(c.Type)))
			{
				builder.AppendLine($"claim -> type : {claim.Type}, value : {claim.Value}");
			}

			return builder.ToString();
		}

		public static IEnumerable<int> GetOrgIds(this ClaimsPrincipal principal)
		{
			return principal.GetIds(SecurityConstants.ORG_ID);
		}

		public static IEnumerable<int> GetDivIds(this ClaimsPrincipal principal)
		{
			return principal.GetIds(SecurityConstants.DIV_ID);
		}

		public static bool ComesFromClient(this ClaimsPrincipal principal, string clientId)
		{
			return principal.HasClaim(c => c.Type == SecurityConstants.CLIENT_CLAIM) && 
				   principal.HasClaim(SecurityConstants.CLIENT_CLAIM, clientId);
		}

		private static IEnumerable<int> GetIds(this ClaimsPrincipal principal, string pattern)
		{
			// authorization claims will be in the 'prefix':'numeric id' pattern.  ex: orgId:5 or divId:123456
			return principal
				.Claims
				.Where(c => c.Value.IndexOf(pattern) >= 0)
				.Select(c =>
				{
					string[] split = c.Value.Split(':');

					if (split != null && split.Length == 2 && int.TryParse(split[1], out int value))
					{
						return value;
					}

					return -1;
				})
				.Where(i => i > -1);
		}

		private static bool FindClaim(this ClaimsPrincipal principal, string group, int id, string claimPrefix)
		{
			// authorization claims will be in the 'prefix':'numeric id' pattern.  ex: orgId:5 or divId:123456
			return principal.HasClaim(c => c.Type == group && c.Value == $"{claimPrefix}:{id}");
		}
	}
}