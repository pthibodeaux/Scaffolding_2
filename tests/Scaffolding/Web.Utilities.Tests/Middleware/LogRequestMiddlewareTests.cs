using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Scaffolding.NLog;
using Xunit;
using Scaffolding.Web.Utilities.Middleware;
using NL = NLog;

namespace Scaffolding.Web.Utilities.Tests.Middleware
{
	public class LogRequestMiddlewareTests
	{
		private readonly Mock<ILoggerFactory> _mockLoggerFactory;
		private readonly Mock<ILogger> _mockLogger;
		private MemoryStream _requestBodyStream;

		public LogRequestMiddlewareTests()
		{
			_mockLoggerFactory = new Mock<ILoggerFactory>();
			_mockLogger = new Mock<ILogger>();
			_mockLoggerFactory.Setup(logger => logger.CreateLogger(It.IsAny<string>()))
				.Returns(_mockLogger.Object);
		}

		//Unit tests for LogRequestMiddleware
		[Fact]
		public async Task LogRequestMiddleware_Info_Logger_Called_For_Info_Only()
		{
			var LogRequestMiddleware = new LogRequestMiddleware(
				async (innerHttpContext) =>
				{ await innerHttpContext.Response.WriteAsync("test response body"); },
				_mockLoggerFactory.Object);

			var path = "/api/";
			var bodyText = "request body text";
			var context = CreateContext(path, bodyText);

			await LogRequestMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), 
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains(path)),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains("UserClaim")),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_requestBodyStream.Dispose();
		}

		[Fact]
		public async Task LogRequestMiddleware_Debug_Logger_Called_For_Info_And_Debug()
		{
			NLogConfig.TrackDebugEnabled("LogRequestMiddleware", NL.LogLevel.Debug);

			var LogRequestMiddleware = new LogRequestMiddleware(
				async (innerHttpContext) =>
				{ await innerHttpContext.Response.WriteAsync("test response body"); },
				_mockLoggerFactory.Object);

			var path = "/api/";
			var bodyText = "request body text";
			var context = CreateContext(path, bodyText);
			context.Request.ContentLength = bodyText.Length;

			await LogRequestMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(),
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains(path)),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains("UserClaim")),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains(bodyText)),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_requestBodyStream.Dispose();

		}

		[Fact]
		public async Task LogRequestMiddleware_Logger_NotCalled()
		{
			var LogRequestMiddleware = new LogRequestMiddleware(
				async (innerHttpContext) =>
				{ await innerHttpContext.Response.WriteAsync("test response body"); },
				_mockLoggerFactory.Object);

			var context = CreateContext("/filename/", "request body text");

			await LogRequestMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), 
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), 
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never);

			_requestBodyStream.Dispose();
		}


		private DefaultHttpContext CreateContext(string path, string bodyText)
		{
			var context = new DefaultHttpContext();
			context.Request.Path = path;
			context.Request.Scheme = "http";
			context.Request.Protocol = "HTTP/1.1";
			context.Request.Host = new HostString("localhost:5010");
			context.Request.Method = "GET";

			byte[] byteArray = Encoding.UTF8.GetBytes(bodyText);
			_requestBodyStream = new MemoryStream(byteArray);

			context.Request.Body = _requestBodyStream;
			return context;
		}

	}
}
