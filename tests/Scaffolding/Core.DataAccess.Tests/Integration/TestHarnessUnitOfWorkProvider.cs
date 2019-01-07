using System.Data;
using System.Data.SqlClient;

namespace Scaffolding.Core.DataAccess.Tests.Integration
{
    public class TestHarnessUnitOfWorkProvider : BaseProvider<TestHarnessDbConfig>
    {
	    public TestHarnessUnitOfWorkProvider(TestHarnessDbConfig config) : base(config) { }

		public override IDbConnection GetConnection()
	    {
		    return new SqlConnection(_config.ConnectionString);
	    }
    }
}
