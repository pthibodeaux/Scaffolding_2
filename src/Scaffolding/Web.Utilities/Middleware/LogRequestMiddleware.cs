using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace Scaffolding.Web.Utilities.Middleware
{
	public class LogRequestMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;
		private readonly string _categoryName;

		public LogRequestMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			_next = next;
			if (loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}

			_categoryName = nameof(LogRequestMiddleware);
			_logger = loggerFactory.CreateLogger(_categoryName);
		}

		public async Task Invoke(HttpContext context)
		{
			var url = UriHelper.GetDisplayUrl(context.Request);

			if (url != null && url.Contains("/api/")) //only log api requests
			{
				_logger.LogInformation($"Request | Method :  {context?.Request.Method}" + $"\tRoute :  {url}");

				LogUserInfo(context);

				//HACK: check with NLOG config to determine whether DEBUG is enabled (no other way to check)
				if (NLog.NLogConfig.IsDebugEnabledFor(_categoryName))
				{
					//only want to intercept the body stream if Debug is enabled 
					await LogRequestBody(context);
					return;
				}
			}
			await _next(context);
		}

		private async Task LogRequestBody(HttpContext context)
		{
			HttpRequest request = context.Request;
			var body = request.Body;

			//allows setting the reader for the request back at the beginning of its stream.
			request.EnableRewind();

			//read the request stream into a string
			var buffer = new byte[Convert.ToInt32(request.ContentLength)];
			await request.Body.ReadAsync(buffer, 0, buffer.Length);
			var bodyAsText = Encoding.UTF8.GetString(buffer);

			_logger.LogDebug($"Request | Body: {bodyAsText}");

			//reset to start of stream
			request.Body.Position = 0;

			//Continue down the Middleware pipeline, eventually returning to this class
			await _next(context);

			// assign the read body back to the request body, which is allowed because of EnableRewind()
			request.Body = body;
		}

		private void LogUserInfo(HttpContext context)
		{
			var userClaims = context.User?.Claims;
			if (userClaims != null)
				_logger.LogInformation("UserClaim | " + userClaims?.Join(" \n "));
		}

	}
}
