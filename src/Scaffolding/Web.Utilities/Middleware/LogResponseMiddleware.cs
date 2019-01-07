using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Scaffolding.Web.Utilities.Middleware
{
	public class LogResponseMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;
		private readonly string _categoryName;

		public LogResponseMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			_next = next;
			if (loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}
			 
			_categoryName = nameof(LogResponseMiddleware);
			_logger = loggerFactory.CreateLogger(_categoryName);
		}

		public async Task Invoke(HttpContext context)
		{
			var url = UriHelper.GetDisplayUrl(context.Request);
			if (url != null && url.Contains("/api/")) //only log api requests
			{
				_logger.LogInformation($"Response | Method :  {context?.Request.Method}" +
				                       $"\tRoute :  {url}" +
				                       $"\tResponse Status Code:  {context?.Response?.StatusCode}");

				//HACK: check with NLOG config to determine whether DEBUG is enabled (no other way to check)
				//only want to mess with the body stream if Debug is enabled 
				if (NLog.NLogConfig.IsDebugEnabledFor(_categoryName))
				{
					await LogResponseBody(context);
					return;
				}
			}

			await _next(context);
		}

		private async Task LogResponseBody(HttpContext context)
		{
			if (context.Response.ContentType == "application/pdf")
			{
				_logger.LogDebug($"Response | Body: [pdf document bytes - not logged]");
				return;
			}

			var originalBodyStream = context.Response.Body;
			using (var responseBody = new MemoryStream())
			{
				context.Response.Body = responseBody;

				//Continue down the Middleware pipeline, eventually returning to this class
				await _next(context);

				//read the response stream into a string
				HttpResponse response = context.Response;

				//reset to start of stream
				response.Body.Position = 0;

				string bodyAsText;
				// NOTE: final 'true' param prevents the closing of the streamreader from closing the stream as well (default behavior) 
				// reference: https://stackoverflow.com/questions/10934585/memorystream-cannot-access-a-closed-stream
				using (var sr = new StreamReader(response.Body, Encoding.UTF8, false, 1024, true))
				{
					bodyAsText = await sr.ReadToEndAsync();
					//reset to start of stream
					response.Body.Position = 0;
				}

				//do not log pdf bytes - no value
				_logger.LogDebug(context.Response.ContentType == "application/pdf"
					? $"Response | Body: [pdf document bytes - not logged]"
					: $"Response | Body: {bodyAsText}");

				await responseBody.CopyToAsync(originalBodyStream);
			}
		}

	}
}
