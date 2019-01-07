using System.Collections.Generic;
using WebApiHub.Models.Common;

namespace Common.TestFixtures
{
	public class AddressTestData
	{
		public static IEnumerable<Address> Addresses => new List<Address>
		{
			new Address
			{
				Address1 = "10155 Westmoor Drive",
				Address2 = "Building 3",
				Rank = 1,
				City = "Westminster",
				State = "CO",
				Country = "US",
				ZipCodeText = "80021"
			},
			new Address
			{
				Address1 = "Test Address 1",
				Address2 = "Test Address 2",
				Rank = 2,
				City = "Superior",
				State = "CO",
				Country = "US",
				ZipCodeText = "80027"
			}
		};
	}
}