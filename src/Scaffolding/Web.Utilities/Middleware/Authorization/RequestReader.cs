using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Scaffolding.Core.Identity;

namespace Scaffolding.Web.Utilities.Middleware.Authorization
{
    public class RequestReader : IRequestReader
	{
	    private readonly JsonSerializer _json = new JsonSerializer();

	    public DataIds GetDataIds(object resource)
	    {
		    if (resource is AuthorizationFilterContext mvcContext)
		    {
			    return ReadRequest(mvcContext.HttpContext.Request);
		    }

		    return null;
	    }

		public DataIds ReadRequest(HttpRequest request)
		{
			if (request == null)
				return null;
		    
		    DataIds ids = null;
			
		    // check posted data
		    if (request.ContentType != null && request.ContentType.Contains("application/json") && request.ContentLength > 0)
		    {
			    request.EnableRewind();
			    ids = _json.Deserialize<DataIds>(new JsonTextReader(new StreamReader(request.Body)));
			    request.Body.Position = 0;
		    }

		    // check querystring
		    if (ids == null && request.Query.Count > 0)
		    {
			    ids = new DataIds
			    {
				    OrganizationId = request.Query[SecurityConstants.ORG_ID],
				    DivisionId = request.Query[SecurityConstants.DIV_ID]
			    };
		    }

		    return ids;
	    }

    }
}
