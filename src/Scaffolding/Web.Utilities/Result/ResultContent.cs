using System;
using System.Collections.Generic;
using System.Net;

namespace Scaffolding.Web.Utilities.Result
{
	public class ResultContent<TModel>
	{
		public TModel ContentModel { get; set; }
		public string ErrorMessage { get; set; }
		public bool IsSuccess { get; set; }
		public string DetailMessage { get; set; }
		public Dictionary<string, string[]> PropertyMessages { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public Exception Exception { get; set; }

		public static ResultContent<TModel> Failure(string errorMessage)
		{
			return new ResultContent<TModel>
			{
				IsSuccess = false,
				ErrorMessage = errorMessage,
				DetailMessage = string.Empty,
				ContentModel = default(TModel),
				PropertyMessages = null,
				StatusCode = HttpStatusCode.InternalServerError,
				Exception = null
			};
		}

		public static ResultContent<TModel> Failure(string errorMessage, string detailMessage = "",
			Dictionary<string, string[]> propertyInfo = null)
		{
			return new ResultContent<TModel>
			{
				IsSuccess = false,
				ErrorMessage = errorMessage,
				DetailMessage = detailMessage,
				ContentModel = default(TModel),
				PropertyMessages = propertyInfo,
				StatusCode = HttpStatusCode.InternalServerError
			};
		}

		public static ResultContent<TModel> Failure(HttpStatusCode statusCode, string errorMessage, string detailMessage = "",
			Exception exception = null)
		{
			return new ResultContent<TModel>
			{
				IsSuccess = false,
				ErrorMessage = errorMessage,
				DetailMessage = detailMessage,
				ContentModel = default(TModel),
				StatusCode = statusCode,
				PropertyMessages = null,
				Exception = exception
			};
		}

		public static ResultContent<TModel> Failure(HttpStatusCode statusCode, string errorMessage,
			Dictionary<string, string[]> propertyInfo, string detailMessage = "", Exception exception = null)
		{
			return new ResultContent<TModel>
			{
				IsSuccess = false,
				ErrorMessage = errorMessage,
				DetailMessage = detailMessage,
				ContentModel = default(TModel),
				StatusCode = statusCode,
				PropertyMessages = propertyInfo,
				Exception = exception
			};
		}

		public static ResultContent<TModel> Failure(Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
		{
			return new ResultContent<TModel>
			{
				IsSuccess = false,
				ErrorMessage = exception.Message,
				DetailMessage = exception.InnerException?.Message,
				ContentModel = default(TModel),
				StatusCode = statusCode,
				PropertyMessages = null,
				Exception = exception
			};
		}

		public static ResultContent<TModel> Success(TModel model)
		{
			return new ResultContent<TModel>
			{
				IsSuccess = true,
				ErrorMessage = string.Empty,
				DetailMessage = string.Empty,
				ContentModel = model,
			};
		}
	}
}
