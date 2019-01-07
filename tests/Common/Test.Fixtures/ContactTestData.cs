using System.Collections.Generic;
using WebApiHub.Models.Organizations;

namespace Common.TestFixtures
{
    public class ContactTestData
    {
	    public static Contact Contact1 => new Contact
	    {
		    Id = 1,
		    Name = "Mary Smith",
		    ContactType = "Manager",
		    IsActive = true,
		    PhoneNumber = "9999999999",
		    Email = "test@email.net",
		    SecondaryPhoneNumber = "8888888888",
		    SecondaryEmail = "test2@email.net"
	    };



		public static IEnumerable<Contact> Contacts =>
			new[]
		    {
			    Contact1,
			    new Contact
				{
					Id = 2,
					Name = "Doug Jones",
					ContactType = "Grunt",
					IsActive = true,
					PhoneNumber = "7777777777",
					Email = "doug@email.net"
			    }
			};
    }
}
