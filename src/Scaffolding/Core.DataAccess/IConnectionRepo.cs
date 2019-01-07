using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
	public interface IConnectionRepo : IBaseRepository
	{
		IConnection UnitOfWork { get; set; }
		IAsyncConnection AsyncUnitOfWork { get; set; }
		string GetSql(string resource);
		string GetSqlFromResource(string resource);
	}
}