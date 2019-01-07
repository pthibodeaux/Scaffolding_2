using System.Collections.Generic;
using WebApiHub.Models.UserAccounts;

namespace Common.TestFixtures
{
	public class UserAccountTestData
	{
		public static List<UserAccount> UserAccounts => new List<UserAccount>
		{
			new UserAccount
			{
				UserAccountId = 1,
				UserId = 1000001,
				IsActive = true,
				FirstName = "Deanna",
				LastName = "Taylor",
				Emails = EmailTestData.Emails,
				Addresses = AddressTestData.Addresses,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers
			},
			new UserAccount
			{
				UserAccountId = 2,
				UserId = 1000002,
				IsActive = false,
				FirstName = "Tonya L.",
				LastName = "Travers",
				Emails = EmailTestData.Emails,
				Addresses = AddressTestData.Addresses,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
            },
			new UserAccount
			{
				UserAccountId = 3,
				UserId = 1000003,
				IsActive = true,
				FirstName = "Vernon L.",
				LastName = "Smith",
				Emails = EmailTestData.Emails,
				Addresses = AddressTestData.Addresses,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
            }
		};
	}
}