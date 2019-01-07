using System.Collections.Generic;

namespace Scaffolding.Core.Identity
{
    public static class SecurityConstants
    {
		public const string ORG_ID = "organizationId";
	    public const string DIV_ID = "divisionId";

	    public const string APP_ADMIN = "app_admin";
	    public const string ORG_ADMIN = "org_admin";
	    public const string DIV_ADMIN = "division_admin";
	    public const string DIV_CONTRIB = "division_contrib";
	    public const string DIV_WATCH = "division_watch";

        public const string CAGE_USER = "cage_user";

	    public const string CLIENT_CLAIM = "client_id";

	    public const string CONSOLE_CLIENT = "client";
	    public const string CRM_CLIENT = "crm";
	    public const string SWAGGER_CLIENT = "swagger";

		public static readonly List<string> AllGroups = new List<string>
	    {
		    APP_ADMIN,
		    ORG_ADMIN,
		    DIV_ADMIN,
		    DIV_CONTRIB,
		    DIV_WATCH,
		    CAGE_USER
	    };

	    public const string IDENTITY_HUB_USER_AUTH_TYPE = "BearerIdentityServerAuthenticationIntrospection";
    }
}
