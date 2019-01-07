using System.Collections.Generic;
using WebApiHub.Models.Common;

namespace Common.TestFixtures
{
	public class EmailTestData
	{
		public static IEnumerable<Email> Emails => new List<Email>
		{
			new Email
			{
				EmailAddress = "xyz@aegispremier.com",
				Rank = 1
			},
			new Email
			{
				EmailAddress = "hello@aegispremier.com",
				Rank = 2
			},
			new Email
			{
				EmailAddress = "test@aegispremier.com",
				Rank = 3
			}
		};
	}
}