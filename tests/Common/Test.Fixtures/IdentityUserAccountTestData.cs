using System.Collections.Generic;
using CRM.Core.Models;
using IdentityHub.Core.Models;
using Scaffolding.Core.Identity;

namespace Common.TestFixtures
{
    public class IdentityUserAccountTestData
    {
		public static List<IdentityUserAccount> IdentityUserAccounts => new List<IdentityUserAccount>
		{
			new IdentityUserAccount
			{
				OrganizationId = 1,
				Username = "oneoneone",
				Email = "one@one.com",
				Password = "password",
				PasswordHash = "sgdsfgsdffdfsf",
				PasswordSalt = "fgsdfgfdsgsdfgsd",
				DivisionRoles = new List<DivisionRole>()
				{
					new DivisionRole()
					{
						DivisionId = 1,
						DivisionName = "Test Division",
						RoleName = SecurityConstants.DIV_CONTRIB
					}
				},
				RoleData = new List<UserRoleData>()
				{
					new UserRoleData()
					{
						DivisionId = 1,
						OrgId = 1,
						RoleName = SecurityConstants.DIV_CONTRIB
					}
				},
				UserAccountId = 5,
				UserId = 5,
				PersonId = 5,
				FirstName = "one",
				MiddleName = "one",
				LastName = "one",
				NickName = "one",
				Emails = EmailTestData.Emails,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				Addresses = AddressTestData.Addresses,
				IsActive = true
			},
			new IdentityUserAccount
			{
				OrganizationId = 1,
				Username = "twotwotwo",
				Email = "two@two.com",
				Password = "password",
				PasswordHash = "hjkghkghjhkgj",
				PasswordSalt = "ssfhsfshshghfgh",
				DivisionRoles = new List<DivisionRole>()
				{
					new DivisionRole()
					{
						DivisionId = 1,
						DivisionName = "Test Division",
						RoleName = SecurityConstants.DIV_CONTRIB
					}
				},
				RoleData = new List<UserRoleData>()
				{
					new UserRoleData()
					{
						DivisionId = 1,
						OrgId = 1,
						RoleName = SecurityConstants.DIV_CONTRIB
					}
				},
				UserAccountId = 10,
				UserId = 10,
				PersonId = 10,
				FirstName = "two",
				MiddleName = "two",
				LastName = "two",
				NickName = "two",
				Emails = EmailTestData.Emails,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				Addresses = AddressTestData.Addresses,
				IsActive = true
			},
			new IdentityUserAccount
			{
				OrganizationId = 1,
				Username = "threethreethree",
				Email = "three@three.com",
				Password = "password",
				PasswordHash = "hjkghkghjhkgj",
				PasswordSalt = "ssfhsfshshghfgh",
				DivisionRoles = new List<DivisionRole>()
				{
					new DivisionRole()
					{
						DivisionId = 1,
						DivisionName = "Test Division",
						RoleName = SecurityConstants.DIV_CONTRIB
					}
				},
				RoleData = new List<UserRoleData>()
				{
					new UserRoleData()
					{
						DivisionId = 1,
						OrgId = 1,
						RoleName = SecurityConstants.DIV_CONTRIB
					}
				},
				UserAccountId = 15,
				UserId = 15,
				PersonId = 15,
				FirstName = "three",
				MiddleName = "three",
				LastName = "three",
				NickName = "three",
				Emails = EmailTestData.Emails,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				Addresses = AddressTestData.Addresses,
				IsActive = true
			}
		};
    }
}
