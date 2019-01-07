using System.Collections.Generic;
using WebApiHub.Models.Common;

namespace Common.TestFixtures
{
	public class PhoneNumberTestData
	{
		public static IEnumerable<PhoneNumber> PhoneNumbers => new List<PhoneNumber>
		{
			new PhoneNumber
			{
				Number = "3334445656",
				PhoneNumberCode = PhoneNumberCode.Voice,
				Rank = 1
			},
			new PhoneNumber
			{
				Number = "1112223333",
				PhoneNumberCode = PhoneNumberCode.Voice,
				Rank = 2
			},
			new PhoneNumber
			{
				Number = "9998887777",
				PhoneNumberCode = PhoneNumberCode.Voice,
				Rank = 3
			}
		};
	}
}