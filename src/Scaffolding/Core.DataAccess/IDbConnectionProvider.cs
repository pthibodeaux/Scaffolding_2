using System.Data;

namespace Scaffolding.Core.DataAccess
{
    public interface IDbConnectionProvider
    {
	    IDbConnection GetConnection();
	}
}
