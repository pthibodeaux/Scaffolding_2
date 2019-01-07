using System.Collections.Generic;
using WebApiHub.Models.Organizations;

namespace Common.TestFixtures
{
    public class DivisionTestData
    {
	    public static Division Division1 => new Division
	    {
		    Id = 1,
		    OrganizationId = OrganizationTestData.Org0Id,
			Name = "Test Division",
		    Abbreviation = "Test_Div",
		    WebSite = "www.testorg.com",
		    CustomCodeText = "DefaultDiv",
		    Country = "USA",
		    IsActive = true,
		    IsPrimary = true,
		    Contacts = ContactTestData.Contacts,
		    Emails = EmailTestData.Emails,
		    PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
		    Addresses = AddressTestData.Addresses,
		    FiscalStartMonthCode = 1
	    };

		public static List<Division> Divisions => new List<Division>(new[]
	    {
		    DivisionTestData.Division1,
		    new Division
		    {
			    Id = 2,
			    OrganizationId = OrganizationTestData.Org0Id,
				Name = "Test Division2",
			    Abbreviation = "Test_Div_2",
			    WebSite = "www.testorg_Div2.com",
			    CustomCodeText = "Org1Div2",
			    Country = "USA",
			    IsActive = true,
			    IsPrimary = true,
			    Contacts = ContactTestData.Contacts,
			    Emails = EmailTestData.Emails,
			    PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
			    Addresses = AddressTestData.Addresses,
				FiscalStartMonthCode = 1
		    },
		    new Division
		    {
			    Id = 3,
			    OrganizationId = OrganizationTestData.Org1Id,
				Name = "Org2 Test Division1",
			    Abbreviation = "Org2 Test_Div_1",
			    WebSite = "www.testOrg2_Div1.com",
			    CustomCodeText = "Org1Div1",
			    Country = "USA",
			    IsActive = true,
			    IsPrimary = true,
			    Contacts = ContactTestData.Contacts,
			    Emails = EmailTestData.Emails,
			    PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
			    Addresses = AddressTestData.Addresses,
			    FiscalStartMonthCode = 1
		    },
		    new Division
		    {
			    Id = 4,
			    OrganizationId = OrganizationTestData.Org1Id,
				Name = "Org2 Test Division2",
			    Abbreviation = "Org2 Test_Div_2",
			    WebSite = "www.testOrg2_Div2.com",
			    CustomCodeText = "Org1Div2",
			    Country = "USA",
			    IsActive = true,
			    IsPrimary = true,
			    Contacts = ContactTestData.Contacts,
			    Emails = EmailTestData.Emails,
			    PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
			    Addresses = AddressTestData.Addresses,
			    FiscalStartMonthCode = 1
		    }
		});

		public static Division DivisionForUpdate => new Division
		{
			Id = 1,
			Name = "Updated Division Name",
			Abbreviation = "Test_Div_Updated",
			WebSite = "www.testorg.com",
			CustomCodeText = "DefaultDiv",
			Country = "USA",
			IsActive = true,
			IsPrimary = true,
			Contacts = ContactTestData.Contacts,
			Emails = EmailTestData.Emails,
			PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
			Addresses = AddressTestData.Addresses,
			FiscalStartMonthCode = 1
	    };

	}
}
