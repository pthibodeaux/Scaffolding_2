namespace Scaffolding.Web.Utilities.Middleware.Authorization
{
	public interface IRequestReader
	{
		DataIds GetDataIds(object resource);
	}
}