using System;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Scaffolding.Web.Utilities.Filters;
using Scaffolding.Web.Utilities.Result;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Filters
{
	public class ServiceExceptionFilterTests
	{
		private readonly ActionContext _actionContext;
		private readonly Mock<ILoggerFactory> _mockLoggerFactory;

		public ServiceExceptionFilterTests()
		{
			_mockLoggerFactory = new Mock<ILoggerFactory>();

			var mockLogger = new Mock<ILogger>();

			_actionContext = new ActionContext
			{
				HttpContext = new DefaultHttpContext(),
				RouteData = new RouteData(),
				ActionDescriptor = new ActionDescriptor()
			};

			_mockLoggerFactory.Setup(logger => logger.CreateLogger(It.IsAny<string>()))
				.Returns(mockLogger.Object);
		}

		[Fact]
		public void OnException_Should_Set_Context_Result_As_InternalServerError()
		{
			string exceptionMessage = "Some Exception";

			var context = new ExceptionContext(_actionContext, Enumerable.Empty<IFilterMetadata>().ToList())
			{
				Exception = new Exception(exceptionMessage)
			};

			var exceptionFilter = new ServiceExceptionFilter(_mockLoggerFactory.Object);

			exceptionFilter.OnException(context);

			JsonResult jsonResult = context.Result as JsonResult;
			ResultContent<string> result = jsonResult.Value as ResultContent<string>;

			string errorMessage = result.ErrorMessage;
			bool isError = !result.IsSuccess;

			context.Result.Should().BeOfType(typeof(JsonResult));
			context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
			errorMessage.Should().Contain(exceptionMessage);
			isError.Should().BeTrue();
		}

		[Fact]
		public void OnException_Should_Set_Context_Result_As_UnAuthorized()
		{
			var context = new ExceptionContext(_actionContext, Enumerable.Empty<IFilterMetadata>().ToList())
			{
				Exception = new UnauthorizedAccessException()
			};

			var exceptionFilter = new ServiceExceptionFilter(_mockLoggerFactory.Object);

			exceptionFilter.OnException(context);

			JsonResult jsonResult = context.Result as JsonResult;
			ResultContent<string> result = jsonResult.Value as ResultContent<string>;

			string errorMessage = result.ErrorMessage;
			bool isError = !result.IsSuccess;

			context.Result.Should().BeOfType(typeof(JsonResult));
			context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
			errorMessage.Should().NotBeNullOrEmpty();
			isError.Should().BeTrue();
		}

		[Fact]
		public void OnExceptionAsync_Should_Set_Context_Result_As_InternalServerError()
		{
			string exceptionMessage = "Some Exception";

			var context = new ExceptionContext(_actionContext, Enumerable.Empty<IFilterMetadata>().ToList())
			{
				Exception = new Exception(exceptionMessage)
			};

			var exceptionFilter = new ServiceExceptionFilter(_mockLoggerFactory.Object);

			exceptionFilter.OnExceptionAsync(context);

			JsonResult jsonResult = context.Result as JsonResult;
			ResultContent<string> result = jsonResult.Value as ResultContent<string>;

			string errorMessage = result.ErrorMessage;
			bool isError = !result.IsSuccess;

			context.Result.Should().BeOfType(typeof(JsonResult));
			context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
			errorMessage.Should().Contain(exceptionMessage);
			isError.Should().BeTrue();
		}

		[Fact]
		public void OnExceptionAsync_Should_Set_Context_Result_As_UnAuthorized()
		{
			var context = new ExceptionContext(_actionContext, Enumerable.Empty<IFilterMetadata>().ToList())
			{
				Exception = new UnauthorizedAccessException()
			};

			var exceptionFilter = new ServiceExceptionFilter(_mockLoggerFactory.Object);

			exceptionFilter.OnException(context);

			JsonResult jsonResult = context.Result as JsonResult;
			ResultContent<string> result = jsonResult.Value as ResultContent<string>;

			string errorMessage = result.ErrorMessage;
			bool isError = !result.IsSuccess;

			context.Result.Should().BeOfType(typeof(JsonResult));
			context.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
			errorMessage.Should().NotBeNullOrEmpty();
			isError.Should().BeTrue();
		}
	}
}
