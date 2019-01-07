using System;
using System.Collections.Generic;
using WebApiHub.Models.Activities;
using WebApiHub.Models.Donor;

namespace Common.TestFixtures
{
	public class DonorTestData
	{
		public static List<Donor> Donors => new List<Donor>
		{
			new Donor
			{
				DonorId = 1,
				PersonId = 1,
				IsActive = true,
				FirstName = "Deanna",
				LastName = "Taylor",
				Emails = EmailTestData.Emails,
				Addresses = AddressTestData.Addresses,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				ReferenceIdText = "Test21829083",
				ContactMethodCode = ContactMethodCode.Email,
				OriginCodeText = "SomeTestCode",
				OriginDate = new DateTime(2016, 1, 2),
				GenderCode = GenderCode.Female,
				RelationshipStatusCode = RelationshipStatusCode.Married,
				DateOfBirth = new DateTime(1980, 1, 1),
				DivisionId = 1,
				DonorCode = DonorCode.New,
				LifetimeGivingAmount = 111,
				LastResponseAmount = 100,
				LastResponseDate = new DateTime(2018, 2, 2)
			},
			new Donor
			{
				DonorId = 2,
				PersonId = 2,
				IsActive = false,
				FirstName = "Tonya L.",
				LastName = "Travers",
				Emails = EmailTestData.Emails,
				Addresses = AddressTestData.Addresses,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				ReferenceIdText = "Test21829084",
				OriginCodeText = "SomeTestCode",
				OriginDate = new DateTime(2016, 1, 2),
				GenderCode = GenderCode.Female,
				RelationshipStatusCode = RelationshipStatusCode.Single,
				DateOfBirth = new DateTime(1980, 1, 1),
				DivisionId = 1
			},
			new Donor
			{
				DonorId = 3,
				PersonId = 3,
				IsActive = true,
				FirstName = "Vernon L.",
				LastName = "Smith",
				Emails = EmailTestData.Emails,
				Addresses = AddressTestData.Addresses,
				PhoneNumbers = PhoneNumberTestData.PhoneNumbers,
				ReferenceIdText = "Test21829085",
				OriginCodeText = "SomeTestCode",
				OriginDate = new DateTime(2016, 1, 2),
				GenderCode = GenderCode.Male,
				RelationshipStatusCode = RelationshipStatusCode.NotApplicable,
				DateOfBirth = new DateTime(1980, 1, 1),
				DivisionId = 1
			}
		};
	}
}