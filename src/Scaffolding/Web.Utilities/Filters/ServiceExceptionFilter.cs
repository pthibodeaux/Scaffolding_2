using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Scaffolding.Web.Utilities.Result;

namespace Scaffolding.Web.Utilities.Filters
{
	public class ServiceExceptionFilter : ExceptionFilterAttribute
    {
	    private readonly ILogger _logger;

	    public ServiceExceptionFilter(ILoggerFactory logger)
	    {
			if (logger == null)
		    {
			    throw new ArgumentNullException(nameof(logger));
		    }

		    _logger = logger.CreateLogger(nameof(ServiceExceptionFilter));
		}

	    public override Task OnExceptionAsync(ExceptionContext context)
	    {
			HandleException(context);
		    return base.OnExceptionAsync(context);
	    }

	    public override void OnException(ExceptionContext context)
	    {
		    HandleException(context);

		    base.OnException(context);
	    }

	    private void HandleException(ExceptionContext context)
	    {
		    ResultContent<string> serviceError;
		    if (context.Exception is UnauthorizedAccessException)
		    {
			    serviceError = ResultContent<string>.Failure(HttpStatusCode.Unauthorized, "Unauthorized Access");
			    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			    _logger.LogError(serviceError.ToString());
		    }
		    else
		    {
			    serviceError = ResultContent<string>.Failure(HttpStatusCode.InternalServerError, context.Exception?.Message, "", context.Exception);
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			    _logger.LogError(serviceError.ToString());
		    }

		    context.Result = new JsonResult(serviceError);
	    }
    }
}
