using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Scaffolding.Web.Utilities.Filters;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Filters
{
	public class SecurityHeadersAttributeTests
	{
		private readonly ActionContext _actionContext;

		public SecurityHeadersAttributeTests()
		{
			_actionContext = new ActionContext
			{
				HttpContext = new DefaultHttpContext(),
				RouteData = new RouteData(),
				ActionDescriptor = new ActionDescriptor()
			};
		}

		[Fact]
		public void OnResultExecuting_Should_Add_Headers()
		{
			var context = new ResultExecutingContext(_actionContext, Enumerable.Empty<IFilterMetadata>().ToList(), new ViewResult(), new { });
			var headersAttribute = new SecurityHeadersAttribute();

			headersAttribute.OnResultExecuting(context);

			context.HttpContext.Response.Headers.Should().ContainKey("X-Content-Type-Options");
			context.HttpContext.Response.Headers.Should().ContainKey("X-Frame-Options");
			context.HttpContext.Response.Headers.Should().ContainKey("Content-Security-Policy");
			context.HttpContext.Response.Headers.Should().ContainKey("X-Content-Security-Policy");
		}
	}
}
