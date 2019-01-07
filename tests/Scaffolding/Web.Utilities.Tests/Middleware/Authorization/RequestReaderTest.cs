using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Scaffolding.Core.Identity;
using Scaffolding.Web.Utilities.Middleware.Authorization;
//using WebApiHub.Models.Activities;
//using WebApiHub.Models.Donor;
//using WebApiHub.Models.Organizations;
//using WebApiHub.Models.UserAccounts;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Middleware.Authorization
{
    public class RequestReaderTest
    {
	    private readonly RequestReader _reader;

	    public RequestReaderTest()
	    {
		    _reader = new RequestReader();
	    }

	    private AuthorizationFilterContext SetupResource()
	    {
		    DefaultHttpContext httpCtx = new DefaultHttpContext();

		    ActionContext actionCtx = new ActionContext(httpCtx, new RouteData(), new ActionDescriptor());
		    return new AuthorizationFilterContext(actionCtx, new List<IFilterMetadata>());
	    }

	    private HttpRequest SetupJsonRequest(object request)
	    {
		    DefaultHttpContext httpCtx = new DefaultHttpContext();
			DefaultHttpRequest httpReq = new DefaultHttpRequest(httpCtx);

		    // convert string to stream
		    byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
			httpReq.Body = new MemoryStream(byteArray);
		    httpReq.ContentType = "application/json";
		    httpReq.ContentLength = byteArray.Length;

			return httpReq;
	    }

	    private HttpRequest SetupGetRequest(string[] parts = null)
	    {
		    DefaultHttpContext httpCtx = new DefaultHttpContext();
		    DefaultHttpRequest httpReq = new DefaultHttpRequest(httpCtx);

		    if (parts != null)
		    {
			    StringBuilder builder = new StringBuilder("?");

			    for (int i = 0; i < parts.Length; i++)
			    {
				    builder.Append(parts[i]);
				    builder.Append(i % 2 == 0 ? "=" : "&");
			    }

			    httpReq.QueryString = new QueryString(builder.ToString());
		    }

		    return httpReq;
	    }

	    [Fact]
	    public void ReadRequest_Should_Get_Ids_From_Correct_QueryString()
	    {
		    HttpRequest req = SetupGetRequest(new [] { SecurityConstants.ORG_ID, "5", SecurityConstants.DIV_ID, "10" }); 
		    DataIds parsed = _reader.ReadRequest(req);

		    parsed.OrgNumber.Should().Be(5);
		    parsed.DivNumber.Should().Be(10);
	    }

	    [Fact]
	    public void ReadRequest_Should_Read_Zeros_From_Correct_QueryString_Bad_Ids()
	    {
		    HttpRequest req = SetupGetRequest(new [] { SecurityConstants.ORG_ID, "asdf", SecurityConstants.DIV_ID, "fdsa" });
		    DataIds parsed = _reader.ReadRequest(req);

		    parsed.OrgNumber.Should().Be(0);
		    parsed.DivNumber.Should().Be(0);
	    }

	    [Fact]
	    public void ReadRequest_Should_Read_Zeros_From_Correct_QueryString_Bad_Param_Names()
	    {
		    HttpRequest req = SetupGetRequest(new [] { "OrgId", "5", "DivId", "10" });
		    DataIds parsed = _reader.ReadRequest(req);

		    parsed.OrgNumber.Should().Be(0);
		    parsed.DivNumber.Should().Be(0);
	    }

	    [Fact]
	    public void ReadRequest_Should_Return_Null_From_Null_QueryString()
	    {
		    HttpRequest req = SetupGetRequest();
		    DataIds parsed = _reader.ReadRequest(req);

		    parsed.Should().BeNull();
	    }

	    [Fact]
	    public void ReadRequest_Should_Read_Zeros_From_Incomplete_QueryString()
	    {
		    HttpRequest req = SetupGetRequest(new [] { SecurityConstants.ORG_ID, "5" });
		    DataIds parsed = _reader.ReadRequest(req);
			
		    parsed.OrgNumber.Should().Be(5);
		    parsed.DivNumber.Should().Be(0);
	    }

	    [Fact]
	    public void ReadRequest_Should_Read_Zeros_From_Invalid_QueryString()
	    {
		    HttpRequest req = SetupGetRequest(new [] { SecurityConstants.ORG_ID, "5", "hekllk" });
		    DataIds parsed = _reader.ReadRequest(req);
			
		    parsed.OrgNumber.Should().Be(5);
		    parsed.DivNumber.Should().Be(0);
	    }

	    public static TheoryData<object, int, int> Data
	    {
		    get
		    {
				// removing references to WEBAPI models
			    TheoryData<object, int, int> data = new TheoryData<object, int, int>();
			    //data.Add(new Organization { Id = 5 }, 5, 0);
			    //data.Add(new OrganizationCriteria { OrganizationId = 15 }, 15, 0);
			    data.Add(new Division { Id = 5, OrganizationId = 15 }, 15, 5);
			    //data.Add(new DivisionCriteria { DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    //data.Add(new Donor { Id = 5, DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    //data.Add(new DonorCriteria { DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    //data.Add(new UserAccount { Id = 5, OrganizationId = 15 }, 15, 0);
			    //data.Add(new UserAccountCriteria { DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    //data.Add(new TouchPoint { Id = 5, DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    //data.Add(new Response { DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    //data.Add(new ActivityCriteria { DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    //data.Add(new AnalyticsCriteria { DivisionId = 10, OrganizationId = 15 }, 15, 10);
			    data.Add(new { will = 0, not = 1, find = 2 }, 0, 0);
			    data.Add(new { }, 0, 0);

			    return data;
		    }
	    }

	    [Theory]
	    [MemberData(nameof(Data))]
	    public void ReadRequest_Should_Read_Ids_From_Json(object arg, int org, int div)
	    {
		    HttpRequest req = SetupJsonRequest(arg);
		    DataIds parsed = _reader.ReadRequest(req);
			
		    parsed.OrgNumber.Should().Be(org);
		    parsed.DivNumber.Should().Be(div);
	    }

	    [Fact]
	    public void ReadRequest_Should_Return_Null_When_Wrong_ContentType()
	    {
		    HttpRequest req = SetupJsonRequest(new Division { Id = 5, OrganizationId = 15 });
		    req.ContentType = "text/html";
		    DataIds parsed = _reader.ReadRequest(req);

		    parsed.Should().BeNull();
	    }
    }

	// example model, basically similar to WEBAPI model
	public class Division 
	{
		public int Id
		{
			get => DivisionId;
			set => DivisionId = value;
		}
		public int DivisionId { get; set; }
		public int OrganizationId { get; set; }
	}

}
