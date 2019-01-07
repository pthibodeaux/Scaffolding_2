using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Scaffolding.Web.Utilities.Filters;
using Scaffolding.Web.Utilities.Result;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Filters
{
	public class ValidateModelAttributeTests
	{
		private readonly ActionContext _actionContext;

		public ValidateModelAttributeTests()
		{
			_actionContext = new ActionContext
			{
				HttpContext = new DefaultHttpContext(),
				RouteData = new RouteData(),
				ActionDescriptor = new ActionDescriptor()
			};
		}

		[Fact]
		public void OnResultExecuting_Should_Set_Result_As_BadRequest_For_Model_Errors()
		{
			var context = new ResultExecutingContext(_actionContext, Enumerable.Empty<IFilterMetadata>().ToList(),
				new ContentResult(), new { });
			var validateModelAttribute = new ValidateModelAttribute();

			context.ModelState.AddModelError("testError", "errorMessage");
			validateModelAttribute.OnResultExecutionAsync(context, null);
			context.Result.Should().BeOfType(typeof(BadRequestObjectResult));

			var badresult = (BadRequestObjectResult) context.Result;
			badresult.Value.Should().NotBeNull();
			badresult.Value.Should().BeOfType(typeof(ResultContent<>));
			var resultContent = (ResultContent<string>) badresult.Value;
			resultContent.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			resultContent.ContentModel.Should().BeNull();
			resultContent.PropertyMessages.Should().NotBeEmpty();
			resultContent.PropertyMessages.Should().ContainKey("testError");
			resultContent.PropertyMessages.Count.Should().BeGreaterOrEqualTo(1);
		}
	}
}
