using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Scaffolding.Web.Utilities.Middleware;

namespace Scaffolding.Web.Utilities.Tests.Middleware
{
	public class ErrorHandlingMiddlewareTests
	{
		private readonly Mock<ILoggerFactory> _mockLoggerFactory;

		public ErrorHandlingMiddlewareTests()
		{
			_mockLoggerFactory = new Mock<ILoggerFactory>();
			var mockLogger = new Mock<ILogger>();
			_mockLoggerFactory.Setup(logger => logger.CreateLogger(It.IsAny<string>()))
				.Returns(mockLogger.Object);
		}

		//Unit tests for Error Handling Middleware
		[Fact]
		public async Task Error_Handling_Middleware_Success()
		{
			var errorHandlingMiddleware = new ErrorHandlingMiddleware(
				async (innerHttpContext) =>
				{
					await innerHttpContext.Response.WriteAsync("test response body");
				}, _mockLoggerFactory.Object);

			var context = new DefaultHttpContext();
			await errorHandlingMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
		}

		[Fact]
		public async Task Error_Handling_Middleware_Fail400()
		{
			var errorHandlingMiddleware = new ErrorHandlingMiddleware(
				(innerHttpContext) => throw new ApplicationException("Application exception")
				, _mockLoggerFactory.Object);

			var context = new DefaultHttpContext();
			await errorHandlingMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Error_Handling_Middleware_Fail500()
		{
			var errorHandlingMiddleware = new ErrorHandlingMiddleware(
				(innerHttpContext) => throw new Exception("Server exception")
				, _mockLoggerFactory.Object);

			var context = new DefaultHttpContext();
			await errorHandlingMiddleware.Invoke(context);

			context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
		}

	}
}
