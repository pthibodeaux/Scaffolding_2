using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Scaffolding.Core.DataAccess
{
    public static class ExtentionMethods
    {
	    public static async Task OpenAsync(this IDbConnection instance)
	    {
		    if (instance is SqlConnection connection)
		    {
			    if (connection.State != ConnectionState.Open)
			    {
					await connection.OpenAsync();
			    }
		    }
	    }
    }
}
