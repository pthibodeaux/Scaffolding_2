using System;
using System.Net;
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
	public class LogResponseMiddlewareTests
	{
		private readonly Mock<ILoggerFactory> _mockLoggerFactory;
		private readonly Mock<ILogger> _mockLogger;

		public LogResponseMiddlewareTests()
		{
			_mockLoggerFactory = new Mock<ILoggerFactory>();
			_mockLogger = new Mock<ILogger>();
			_mockLoggerFactory.Setup(logger => logger.CreateLogger(It.IsAny<string>()))
				.Returns(_mockLogger.Object);
		}

		//Unit tests for LogResponseMiddleware
		[Fact]
		public async Task LogResponseMiddleware_Info_Logger_Called_For_Info_Only()
		{
			var path = "/api/";
			var bodyText = "response body text";

			var LogResponseMiddleware = new LogResponseMiddleware(
				async (innerHttpContext) =>
				{ await innerHttpContext.Response.WriteAsync(bodyText); },
				_mockLoggerFactory.Object);

			var context = CreateContext(path);

			await LogResponseMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains(path)),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), 
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never);

		}

		[Fact]
		public async Task LogResponseMiddleware_Debug_Logger_Called_For_Info_And_Debug()
		{
			var path = "/api/";
			var bodyText = "response body text";

			NLogConfig.TrackDebugEnabled("LogResponseMiddleware", NL.LogLevel.Debug);

			var LogResponseMiddleware = new LogResponseMiddleware(
				async (innerHttpContext) =>
				{ await innerHttpContext.Response.WriteAsync(bodyText); },
				_mockLoggerFactory.Object);

			var context = CreateContext(path);

			await LogResponseMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains(path)),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), 
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(),
				It.Is<FormattedLogValues>(v => v.ToString().Contains(bodyText)),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

		}

		[Fact]
		public async Task LogResponseMiddleware_Logger_NotCalled()
		{
			var path = "/filename/";
			var bodyText = "";
			var LogResponseMiddleware = new LogResponseMiddleware(
				async (innerHttpContext) =>
				{ await innerHttpContext.Response.WriteAsync(bodyText); },
				_mockLoggerFactory.Object);

			var context = CreateContext(path);

			await LogResponseMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);

			_mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), 
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never);

			_mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(),
				It.IsAny<FormattedLogValues>(),
				It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never);

		}


		private DefaultHttpContext CreateContext(string path)
		{
			var context = new DefaultHttpContext();
			context.Request.Path = path;
			context.Request.Scheme = "http";
			context.Request.Protocol = "HTTP/1.1";
			context.Request.Host = new HostString("localhost:5010");
			context.Request.Method = "GET";
			return context;
		}

	}
}
