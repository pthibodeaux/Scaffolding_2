using System;
using System.Collections.Generic;
using WebApiHub.Models.Organizations;

namespace Common.TestFixtures
{
	public class OrganizationTestData
	{
		public static readonly int Org0Id = 1;
		public static readonly int Org1Id = 2;

		public static List<Organization> Organizations => new List<Organization>(new[]
		{
			new Organization
			{
				Id = 1,
				Name = "Test Organization " + Guid.NewGuid(),
				Abbreviation = "Test_Org asdf",
				WebSite = "www.testorg.com",
				Country = "USA",
				Contacts = ContactTestData.Contacts,
				Divisions =  new List<Division>(){ DivisionTestData.Division1 },
				Emails = EmailTestData.Emails,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				Addresses = AddressTestData.Addresses,
			},
			new Organization
			{
				Id = 2,
				Name = "Test Organization 2 " + Guid.NewGuid(),
				Abbreviation = "Test_Org_2 asdf",
				Country = "USA",
				Contacts = ContactTestData.Contacts,
				Divisions = new List<Division>{ DivisionTestData.Divisions[1], DivisionTestData.Divisions[2] },
				Emails = EmailTestData.Emails,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				Addresses = AddressTestData.Addresses,
			},
			new Organization
			{
				Id = 2,
				Name = "Test Organization 3 " + Guid.NewGuid(),
				Abbreviation = "Test_Org_3 asdf",
				WebSite = "www.testorg3.com",
				Country = "USA",
				Contacts = ContactTestData.Contacts,
				Divisions = DivisionTestData.Divisions,
				Emails = EmailTestData.Emails,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				Addresses = AddressTestData.Addresses,
			},
			new Organization
			{
				Id = 4,
				Name = "Test Organization 4 " + Guid.NewGuid(),
				Abbreviation = "Test_Org_4 asdf",
				WebSite = "www.testorg4.com",
				Country = "USA",
				Contacts = ContactTestData.Contacts,
				Divisions = DivisionTestData.Divisions,
				Emails = EmailTestData.Emails,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				Addresses = AddressTestData.Addresses,
			}
        });

		public static Organization OrganizationForUpdate => new Organization()
		{
			Id = 1,
			Name = "Updated Test Organization " + Guid.NewGuid(),
			Abbreviation = "Test_Org_Updated asdf",
			WebSite = "www.testorg_Updated.com",
			Country = "USA",
			Contacts = ContactTestData.Contacts,
			Divisions = DivisionTestData.Divisions,
			Emails = EmailTestData.Emails,
			PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
			Addresses = AddressTestData.Addresses,
		};

	}
}
