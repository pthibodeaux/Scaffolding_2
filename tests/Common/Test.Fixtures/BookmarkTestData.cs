using System.Collections.Generic;
using WebApiHub.Models.Bookmarks;

namespace Common.TestFixtures
{
    public class BookmarkTestData
    {
	    public static Bookmark Bookmark1 => new Bookmark
	    {
		    Id = 1,
		    OrganizationId = OrganizationTestData.Org0Id,
		    DivisionId = DivisionTestData.Division1.DivisionId,
		    UserId = 1,
			Name = "Test Bookmark",
		    Description = "Test Bookmark",
		    DashboardTypeId = 1,
		    CriteriaJson = "{}",
		};


		public static List<Bookmark> Bookmarks => new List<Bookmark>(new[]
	    {
		    BookmarkTestData.Bookmark1,
		    new Bookmark
		    {
			    Id = 2,
			    OrganizationId = OrganizationTestData.Org0Id,
			    DivisionId = DivisionTestData.Division1.DivisionId,
			    UserId = 1,
			    Name = "Test Bookmark",
			    Description = "Test Bookmark",
			    DashboardTypeId = 1,
			    CriteriaJson = "{}",
		    }
		});

	}
}
