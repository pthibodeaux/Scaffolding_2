using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Scaffolding.Core.DataAccess.Tests.Integration
{
    public class TestHarnessRepository : BaseRepository
    {
	    public const string SELECT_ALL = @"
select 
	clients.[ClientId] as 'Id',
	clients.[ClientKey] as 'Name'
from [Clients] clients
";
	    public const string SELECT_ONE = @"
clients.[ClientId] = @theId
";
	    public const string SELECT_BY_NAME = @"
clients.[ClientKey] = @theName
";
		public const string SELECT_CHILDREN_FOR_PARENT = @"
select 
	uris.[ClientRedirectUriId],
	uris.[ClientId],
	uris.[Uri]
from [ClientRedirectUris] uris
where uris.[ClientId] = @theId
";

		public TestHarnessRepository(TestHarnessUnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider) { }

	    public IEnumerable<ParentModel> GetAllParents()
	    {
		    return Query<ParentModel>(SELECT_ALL);
	    }

	    public async Task<IEnumerable<ParentModel>> GetAllParentsAsync()
	    {
		    return await QueryAsync<ParentModel>(SELECT_ALL);
		}

	    public ParentModel GetOnlyParentByName(string name)
	    {
		    object theParams = new { theName = name };
		    ParentModel model = QuerySingle<ParentModel>($"{SELECT_ALL} where {SELECT_BY_NAME}", theParams);

		    return model;
	    }

	    public async Task<ParentModel> GetOnlyParentByNameAsync(string name)
		{
			object theParams = new { theName = name };
			ParentModel model = await QuerySingleAsync<ParentModel>($"{SELECT_ALL} where {SELECT_BY_NAME}", theParams);

			return model;
		}

		public ParentModel GetParentById(int unique) 
	    {
			object theParams = new { theId = unique };
			ParentModel model = QuerySingle<ParentModel>($"{SELECT_ALL} where {SELECT_ONE}", theParams);
		    IEnumerable<ClientRedirectUri> children = Query<ClientRedirectUri>(SELECT_CHILDREN_FOR_PARENT, theParams);

		    model.Children = children;

		    return model;
		}

		public async Task<ParentModel> GetParentByIdAsync(int unique)
	    {
			object theParams = new { theId = unique };
		    ParentModel model = await QuerySingleAsync<ParentModel>($"{SELECT_ALL} where {SELECT_ONE}", theParams);
		    IEnumerable<ClientRedirectUri> children = await QueryAsync<ClientRedirectUri>(SELECT_CHILDREN_FOR_PARENT, theParams);

		    model.Children = children;

		    return model;
		}

	    public async Task<object> InsertChildAsync(ClientRedirectUri child)
	    {
		    return await InsertAsync(child);
		}

	    public object InsertChild(ClientRedirectUri child)
	    {
		    return Insert(child);
		}

	    public async Task<(IEnumerable<ParentModel> list, ParentModel single, ParentModel full)> DbOperationAsync(string clientName)
	    {
		    IEnumerable<ParentModel> list = null;
		    ParentModel single = null;
		    ParentModel full = null;

			await UseConnectionAsync(async (db, tran) =>
		    {
			    list = await db.QueryAsync<ParentModel>(new CommandDefinition(SELECT_ALL, null, tran));

			    ParentModel model = list.First(c => c.Name == clientName);
				object nameParams = new { theName = model.Name };

				single = await db.QuerySingleAsync<ParentModel>($"{SELECT_ALL} where {SELECT_BY_NAME}", nameParams, tran);
				full = await GetParentByIdAsync(model.Id);
			});

			return (list, single, full);
	    }

	    public (IEnumerable<ParentModel> list, ParentModel single, ParentModel full) DbOperation(string clientName)
	    {
		    IEnumerable<ParentModel> list = null;
		    ParentModel single = null;
		    ParentModel full = null;

		    UseConnection((db, tran) =>
		    {
			    list = db.Query<ParentModel>(new CommandDefinition(SELECT_ALL, null, tran));

			    object nameParams = new { theName = list.First(c => c.Name == clientName).Name };
			    single = db.QuerySingle<ParentModel>($"{SELECT_ALL} where {SELECT_BY_NAME}", nameParams, tran);
			    full = GetParentById(single.Id);
		    });

		    return (list, single, full);
		}
	}
}
