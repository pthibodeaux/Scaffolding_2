using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Scaffolding.Web.Utilities.Result;

namespace Scaffolding.Web.Utilities.Filters
{
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		public override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			if (context.ModelState.IsValid)
				return base.OnResultExecutionAsync(context, next);

			if (context.ModelState.Keys.Any())
			{
				Dictionary<string, string[]> errors = context
					.ModelState
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());

				var resultContent = ResultContent<string>.Failure(HttpStatusCode.BadRequest, "Validation Errors", errors);
				context.Result = new BadRequestObjectResult(resultContent);
			}
			else
			{
				context.Result = new BadRequestObjectResult(context.ModelState);
			}

			return base.OnResultExecutionAsync(context, next);
		}
	}
}
