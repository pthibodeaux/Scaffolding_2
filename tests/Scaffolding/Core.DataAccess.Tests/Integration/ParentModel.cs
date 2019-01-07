using System.Collections.Generic;

namespace Scaffolding.Core.DataAccess.Tests.Integration
{
    public class ParentModel
    {
	    public int Id { get; set; }
	    public string Name { get; set; }
	    public IEnumerable<ClientRedirectUri> Children { get; set; }
    }
}
