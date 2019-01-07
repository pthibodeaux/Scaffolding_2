namespace Scaffolding.Web.Utilities.Middleware.Authorization
{
    public class DataIds
    {
	    private int _orgId = 0;
	    private int _divId = 0;

	    public string OrganizationId
	    {
		    get => _orgId.ToString();
		    set => int.TryParse(value, out _orgId);
	    }
	    public string DivisionId
	    {
		    get => _divId.ToString();
		    set => int.TryParse(value, out _divId);
	    }

	    public int OrgNumber => _orgId;
	    public int DivNumber => _divId;
    }
}
