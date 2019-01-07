using System.Collections.Generic;
using WebApiHub.Models.Common;
using WebApiHub.Models.People;

namespace Common.TestFixtures
{
	public class PersonTestData
	{
		public static IEnumerable<Person> People => new List<Person>
		{
			new Person
			{
				IsActive = true,
				FirstName = "Deanna",
				LastName = "Taylor",
				Emails = new List<Email>
				{
					new Email
					{
						EmailAddress = "email@provider.com"
					}
				},
				Addresses = new List<Address>
				{
					new Address
					{
						Address1 = "10155 Westmoor Dr",
						City = "Westminster",
						State = "CO",
						ZipCodeText = "80021",
						Country = "US"
					}
				},
				PhoneNumbers = new List<PhoneNumber>
				{
					new PhoneNumber
					{
						Number = "720-000-0000"
					}
				}
			},
			new Person
			{
				IsActive = false,
				FirstName = "Tonya L.",
				LastName = "Travers",
				Emails = new List<Email>
				{
					new Email
					{
						EmailAddress = "email@provider.com"
					}
				},
				Addresses = new List<Address>
				{
					new Address
					{
						Address1 = "10155 Westmoor Dr",
						City = "Westminster",
						State = "CO",
						ZipCodeText = "80021",
						Country = "US"
					}
				},
				PhoneNumbers = new List<PhoneNumber>
				{
					new PhoneNumber
					{
						Number = "720-000-0000"
					}
				}
			},
			new Person
			{
				IsActive = true,
				FirstName = "Vernon L.",
				LastName = "Smith",
				Emails = new List<Email>
				{
					new Email
					{
						EmailAddress = "email@provider.com"
					}
				},
				Addresses = new List<Address>
				{
					new Address
					{
						Address1 = "10155 Westmoor Dr",
						City = "Westminster",
						State = "CO",
						ZipCodeText = "80021",
						Country = "US"
					}
				},
				PhoneNumbers = new List<PhoneNumber>
				{
					new PhoneNumber
					{
						Number = "720-000-0000"
					}
				}
			}
		};
	}
}
