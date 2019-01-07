using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using FluentAssertions;
using Scaffolding.Core.Identity;
using Xunit;

namespace Scaffolding.Core.Tests.Identity
{
	public class ClaimsPrincipalExtentionsTests
	{
		private ClaimsPrincipal _principal;
		private IIdentity _identity;

		public ClaimsPrincipalExtentionsTests()
		{
			_principal = null;
			_identity = null;
		}

		private ClaimsPrincipal CreatePrincipal(IEnumerable<Claim> claims, string authType = null, string nameClaim = null)
		{
			_identity = new ClaimsIdentity(claims, authType, nameClaim, null);
			return new ClaimsPrincipal(_identity);
		}

		private ClaimsPrincipal CreatePrincipal(IIdentity identity)
		{
			_identity = identity;
			return new ClaimsPrincipal(_identity);
		}

		[Fact]
		public void IsAdmin_Should_Return_False_When_Not_Admin()
		{
			_principal = CreatePrincipal(new List<Claim>());

			_principal.IsAdmin().Should().BeFalse();
		}

		[Fact]
		public void IsAdmin_Should_Return_True_When_Admin()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.APP_ADMIN, "True")
			});

			_principal.IsAdmin().Should().BeTrue();
		}

		[Fact]
		public void IsOrgAdmin_Should_Return_True_When_User_Has_Group_And_OrgId()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{10}")
			});

			_principal.IsOrgAdmin(10).Should().BeTrue();
		}

		[Fact]
		public void IsOrgAdmin_Should_Return_False_When_User_Doesnt_Have_Group()
		{
			_principal = CreatePrincipal(new List<Claim>());

			_principal.IsOrgAdmin(10).Should().BeFalse();
		}

		[Fact]
		public void IsOrgAdmin_Should_Return_False_When_User_Has_Group_But_Not_OrgId()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{10}")
			});

			_principal.IsOrgAdmin(15).Should().BeFalse();
		}

		[Fact]
		public void HasGroupWithOrg_Should_Return_True_When_User_Has_Group_And_OrgId()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{10}")
			});

			_principal.HasGroupWithOrg(SecurityConstants.ORG_ADMIN, 10).Should().BeTrue();
		}

		[Fact]
		public void HasGroupWithOrg_Should_Return_False_When_User_Doesnt_Have_Group()
		{
			_principal = CreatePrincipal(new List<Claim>());

			_principal.HasGroupWithOrg(SecurityConstants.ORG_ADMIN, 10).Should().BeFalse();
		}

		[Fact]
		public void HasGroupWithOrg_Should_Return_False_When_User_Has_Group_But_Not_OrgId()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{10}")
			});

			_principal.HasGroupWithOrg(SecurityConstants.ORG_ADMIN, 15).Should().BeFalse();
		}

		[Fact]
		public void HasGroupWithDivision_Should_Return_True_When_User_Has_Group_And_OrgId()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:{10}")
			});

			_principal.HasGroupWithDivision(SecurityConstants.DIV_CONTRIB, 10).Should().BeTrue();
		}

		[Fact]
		public void HasGroupWithDivision_Should_Return_False_When_User_Doesnt_Have_Group()
		{
			_principal = CreatePrincipal(new List<Claim>());

			_principal.HasGroupWithDivision(SecurityConstants.DIV_CONTRIB, 10).Should().BeFalse();
		}

		[Fact]
		public void HasGroupWithDivision_Should_Return_False_When_User_Has_Group_But_Not_OrgId()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:{10}")
			});

			_principal.HasGroupWithDivision(SecurityConstants.DIV_CONTRIB, 15).Should().BeFalse();
		}

		[Fact]
		public void IsUserIdentity_Should_Be_True_When_Name_Present()
		{
			ClaimsIdentity identity =
				new ClaimsIdentity(new List<Claim> {new Claim("name", "theName")}, "auth-type", "name", "role");
			ClaimsPrincipal principal = CreatePrincipal(identity);

			identity.Name.Should().Be("theName");
			principal.IsUserIdentiy().Should().BeTrue();
		}

		[Fact]
		public void IsUserIdentity_Should_Be_True_When_AuthenticationType_From_IdentityHub()
		{
			ClaimsIdentity identity = new ClaimsIdentity(new List<Claim> {new Claim("sub", "theName")},
				SecurityConstants.IDENTITY_HUB_USER_AUTH_TYPE, "name", "role");
			ClaimsPrincipal principal = CreatePrincipal(identity);

			identity.Name.Should().BeNullOrEmpty();
			principal.IsUserIdentiy().Should().BeTrue();
		}

		[Fact]
		public void IsUserIdentity_Should_Be_True_When_Name_And_AuthType_Match()
		{
			ClaimsIdentity identity = new ClaimsIdentity(new List<Claim> {new Claim("name", "theName")},
				SecurityConstants.IDENTITY_HUB_USER_AUTH_TYPE, "name", "role");
			ClaimsPrincipal principal = CreatePrincipal(identity);

			identity.Name.Should().Be("theName");
			principal.IsUserIdentiy().Should().BeTrue();
		}

		[Fact]
		public void IsUserIdentity_Should_Be_False_When_Name_And_AuthType_Missing()
		{
			ClaimsIdentity identity =
				new ClaimsIdentity(new List<Claim> {new Claim("sub", "theName")}, "auth-type", "name", "role");
			ClaimsPrincipal principal = CreatePrincipal(identity);

			identity.Name.Should().BeNullOrEmpty();
			principal.IsUserIdentiy().Should().BeFalse();
		}

		[Fact]
		public void ForLogging_Should_Return_Basic_Org_Info_With_Name_And_Email()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:{10}"),
				new Claim("name", "some_name"),
				new Claim("email", "someone@somewhere.com")
			}, "auth-type", "name");

			string logging = _principal.ForLogging();

			logging.Should().Contain("some_name");
			logging.Should().Contain("auth-type");
			logging.Should().Contain("someone@somewhere.com");
			logging.Should().Contain($"{SecurityConstants.DIV_ID}:{10}");
			logging.Should().Contain(SecurityConstants.DIV_CONTRIB);
		}

		[Fact]
		public void ForLogging_Should_Return_Basic_Org_Info_With_No_Name_And_No_Email()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{25}"),
				new Claim("name", "some_name")
			}, "auth_type");

			string logging = _principal.ForLogging();

			logging.Should().NotContain("some_name");
			logging.Should().Contain("auth_type");
			logging.Should().NotContain("someone@somewhere.com");
			logging.Should().Contain($"{SecurityConstants.ORG_ID}:{25}");
			logging.Should().Contain(SecurityConstants.ORG_ADMIN);
		}

	    [Fact]
	    public void GetUserId_Should_Return_Valid_Id()
	    {
	        int testId = 425;
	        _principal = CreatePrincipal(new List<Claim>
	        {
	            new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{25}"),
                new Claim("id", testId.ToString()),
	            new Claim("name", "some_name")
	        }, "auth_type");

	        int id = _principal.GetUserId();

	        id.Should().BePositive();
	        id.Should().NotBe(null);
	        id.Should().Be(testId);
	    }

	    [Fact]
        public void GetUserId_Should_Throw_When_Id_Isnt_Int()
	    {
	        _principal = CreatePrincipal(new List<Claim>
	        {
	            new Claim("id", "herp_derp)imaSTR1NG#")
	        }, "auth_type");

            Assert.Throws<FormatException>(() => _principal.GetUserId());
        }

	    [Fact]
	    public void GetUserId_Should_Throw_When_No_Id_In_Claim()
	    {
	        _principal = CreatePrincipal(new List<Claim>
	        {
	            new Claim("name", "some_name")
            }, "auth_type");

	        Assert.Throws<Exception>(() => _principal.GetUserId());
	    }

        [Fact]
		public void GetOrgIds_Should_Pull_Out_Valid_Orgs()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{5}"),
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{10}"),
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.DIV_ID}:{15}"),
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:non-numeric"),
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}_20"),
				new Claim("name", "some_name")
			}, "auth_type");

			IEnumerable<int> orgs = _principal.GetOrgIds();

			orgs.Should().NotBeNull();
			orgs.Count().Should().Be(2);
			orgs.Should().Contain(5);
			orgs.Should().Contain(10);
			orgs.Should().NotContain(15);
			orgs.Should().NotContain(20);
		}

		[Fact]
		public void GetDivIds_Should_Pull_Out_Valid_Divs()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.DIV_ADMIN, $"{SecurityConstants.DIV_ID}:{5}"),
				new Claim(SecurityConstants.DIV_ADMIN, $"{SecurityConstants.DIV_ID}:{10}"),
				new Claim(SecurityConstants.ORG_ADMIN, $"{SecurityConstants.ORG_ID}:{15}"),
				new Claim(SecurityConstants.DIV_ADMIN, $"{SecurityConstants.DIV_ID}:non-numeric"),
				new Claim(SecurityConstants.DIV_ADMIN, $"{SecurityConstants.DIV_ID}_20"),
				new Claim("name", "some_name")
			}, "auth_type", "name");

			IEnumerable<int> divs = _principal.GetDivIds();

			divs.Should().NotBeNull();
			divs.Count().Should().Be(2);
			divs.Should().Contain(5);
			divs.Should().Contain(10);
			divs.Should().NotContain(15);
			divs.Should().NotContain(20);
		}

		[Fact]
		public void ComesFromClient_Should_Be_True_When_CRM()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:{10}"),
				new Claim("name", "some_name"),
				new Claim("email", "someone@somewhere.com"),
				new Claim(SecurityConstants.CLIENT_CLAIM, SecurityConstants.CRM_CLIENT)
			}, "auth-type", "name");

			bool fromClient = _principal.ComesFromClient(SecurityConstants.CRM_CLIENT);

			fromClient.Should().BeTrue();
		}

		[Fact]
		public void ComesFromClient_Should_Be_False_When_CRM_Claim_But_Asked_For_Console()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:{10}"),
				new Claim("name", "some_name"),
				new Claim("email", "someone@somewhere.com"),
				new Claim(SecurityConstants.CLIENT_CLAIM, SecurityConstants.CRM_CLIENT)
			}, "auth-type", "name");

			bool fromClient = _principal.ComesFromClient(SecurityConstants.CONSOLE_CLIENT);

			fromClient.Should().BeFalse();
		}

		[Fact]
		public void ComesFromClient_Should_Be_False_When_No_Claim_Present()
		{
			_principal = CreatePrincipal(new List<Claim>
			{
				new Claim(SecurityConstants.DIV_CONTRIB, $"{SecurityConstants.DIV_ID}:{10}"),
				new Claim("name", "some_name"),
				new Claim("email", "someone@somewhere.com")
			}, "auth-type", "name");

			bool fromClient = _principal.ComesFromClient(SecurityConstants.CONSOLE_CLIENT);

			fromClient.Should().BeFalse();
		}
	}
}