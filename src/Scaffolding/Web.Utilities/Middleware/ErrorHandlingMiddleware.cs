using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scaffolding.Web.Utilities.Result;

namespace Scaffolding.Web.Utilities.Middleware
{
	public class ErrorHandlingMiddleware
	{
		private readonly ILogger _logger;
		private readonly RequestDelegate next;

		public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory logger)
		{
			this.next = next;
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			//@@@@ TEMP - fix later
			//_logger = logger.CreateLogger(nameof(ErrorHandlingMiddleware));
			_logger = logger.CreateLogger("ServiceErrorHandlingMiddleware");
		}

		public async Task Invoke(HttpContext context /* other scoped dependencies */)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			Console.WriteLine($"ErrorHandlingMiddleware : {exception.Message}");
			var code = HttpStatusCode.InternalServerError; // 500 if unexpected

			//TODO: use custom exceptions for this?
			if (exception is ApplicationException) code = HttpStatusCode.BadRequest;

			//TODO: determine other possible exception type and how to respond - eg,  HttpStatusCode.Unauthorized;

			ResultContent<object> resultContent = ResultContent<object>.Failure(exception.Message);
			resultContent.Exception = exception;
			resultContent.StatusCode = code;

			var result = JsonConvert.SerializeObject(resultContent,
						new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });


			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			return context.Response.WriteAsync(result);
		}
	}
}
