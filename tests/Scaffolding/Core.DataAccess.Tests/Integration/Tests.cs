using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Scaffolding.Core.DI;
using Scaffolding.Core.DI.Repositories;
using Xunit;

namespace Scaffolding.Core.DataAccess.Tests.Integration
{
    public class Tests
    {
	    private readonly TestHarnessUnitOfWorkProvider _provider;
	    private readonly TestHarnessRepository _repo;

	    public Tests()
	    {
		    IConfigurationRoot config = DependencyInjectionWiring.BuildConfig();
		    TestHarnessDbConfig testConfig = new TestHarnessDbConfig();
		    config.GetSection("TestHarnessDbConfig").Bind(testConfig);

		    _provider = new TestHarnessUnitOfWorkProvider(testConfig);
		    _repo = new TestHarnessRepository(_provider);
	    }

	    [Fact]
	    public void GetAllParents_Should_Return_Parents()
	    {
		    IEnumerable<ParentModel> parents = _repo.GetAllParents();

		    parents.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task GetAllParentsAsync_Should_Return_Parents()
	    {
		    IEnumerable<ParentModel> parents = await _repo.GetAllParentsAsync();

		    parents.Should().NotBeNullOrEmpty();
	    }

	    [Fact]
		public void GetParentByGuid_Should_Return_Parent()
		{
			ParentModel parent = _repo.GetOnlyParentByName("crm");
			parent = _repo.GetParentById(parent.Id);

		    parent.Should().NotBeNull();
		    parent.Children.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task GetParentByGuidAsync_Should_Return_Parent()
	    {
			ParentModel parent = await _repo.GetOnlyParentByNameAsync("crm");
		    parent = await _repo.GetParentByIdAsync(parent.Id);

			parent.Should().NotBeNull();
		    parent.Children.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public void Basic_Transaction_Management()
	    {
		    ParentModel initial = null;
		    ParentModel intrans = null;

		    TestHarnessRepository repo2 = new TestHarnessRepository(_provider);
		    TestHarnessRepository repo3 = new TestHarnessRepository(_provider);

		    IUnitOfWork uow = _repo.CreateUnitOfWork(new[] { _repo, repo2, repo3 });

		    using (uow)
		    {
			    try
			    {
				    initial = _repo.GetOnlyParentByName("crm");
				    initial = _repo.GetParentById(initial.Id);
					repo2.InsertChild(new ClientRedirectUri
				    {
					    ClientId = initial.Id,
					    Uri = "asdfasdf"
				    });
				    repo3.InsertChild(new ClientRedirectUri
				    {
					    ClientId = initial.Id,
					    Uri = "fdsafdsa"
				    });
				    intrans = _repo.GetParentById(initial.Id);
			    }
			    finally
			    {
				    uow.RollbackTransaction();
			    }
		    }

		    initial.Should().NotBeNull();
		    intrans.Should().NotBeNull();
		    initial.Children.Should().NotBeNullOrEmpty();
		    intrans.Children.Should().NotBeNullOrEmpty();
		    initial.Children.Count().Should().Be(intrans.Children.Count() - 2);
	    }

	    [Fact]
		public async Task Basic_Transaction_Management_Async()
	    {
		    ParentModel initial = null;
		    ParentModel intrans = null;
		    ParentModel outside = null;
		    int id;

			TestHarnessRepository repo2 = new TestHarnessRepository(_provider);
		    TestHarnessRepository repo3 = new TestHarnessRepository(_provider);

			IAsyncUnitOfWork uow = await _repo.CreateAsyncUnitOfWork(new[] { _repo, repo2, repo3 });

			using (uow)
		    {
			    try
			    {
				    initial = await _repo.GetOnlyParentByNameAsync("crm");
				    id = initial.Id;

				    initial = await _repo.GetParentByIdAsync(id);

					await repo2.InsertChildAsync(new ClientRedirectUri
				    {
					    ClientId = id,
						Uri = "asdfasdf"
				    });
				    await repo3.InsertChildAsync(new ClientRedirectUri
				    {
					    ClientId = id,
					    Uri = "fdsafdsa"
				    });
				    intrans = await _repo.GetParentByIdAsync(id);

				    await _repo.DbOperationAsync("crm");

			    }
			    finally
				{
				    uow.RollbackTransaction();
			    }
			}

		    outside = await _repo.GetParentByIdAsync(id);

			initial.Should().NotBeNull();
		    intrans.Should().NotBeNull();
		    outside.Should().NotBeNull();
		    initial.Children.Should().NotBeNullOrEmpty();
		    intrans.Children.Should().NotBeNullOrEmpty();
		    outside.Children.Should().NotBeNullOrEmpty();
		    initial.Children.Count().Should().Be(intrans.Children.Count() - 2);
		    initial.Children.Count().Should().Be(outside.Children.Count());
		}

		[Fact]
		public async Task Task_WhenAll_Transaction_Management_Async()
	    {
		    ParentModel initial = null;
		    ParentModel intrans = null;
		    ParentModel outside = null;
		    int id;

			TestHarnessRepository repo2 = new TestHarnessRepository(_provider);
		    TestHarnessRepository repo3 = new TestHarnessRepository(_provider);

		    IAsyncUnitOfWork uow = await _repo.CreateAsyncUnitOfWork(new[] { _repo, repo2, repo3 });

		    using (uow)
		    {
			    try
			    {
					initial = await _repo.GetOnlyParentByNameAsync("crm");
				    id = initial.Id;

				    initial = await _repo.GetParentByIdAsync(id);

					List<Task> theKids = new List<Task>(new []
					{
						repo2.InsertChildAsync(new ClientRedirectUri
						{
							ClientId = id,
							Uri = "asdfasdf"
						}),
						repo3.InsertChildAsync(new ClientRedirectUri
						{
							ClientId = id,
							Uri = "fdsafdsa"
						}),
						repo2.InsertChildAsync(new ClientRedirectUri
						{
							ClientId = id,
							Uri = "dsafdsaf"
						}),
						repo3.InsertChildAsync(new ClientRedirectUri
						{
							ClientId = id,
							Uri = "safdsafd"
						})
					});
				    theKids.Add(repo3.DbOperationAsync("crm"));

					await Task.WhenAll(theKids);
				    intrans = await _repo.GetParentByIdAsync(id);
			    }
			    finally
			    {
				    uow.RollbackTransaction();
			    }
		    }

		    outside = await _repo.GetParentByIdAsync(id);

			initial.Should().NotBeNull();
		    intrans.Should().NotBeNull();
		    outside.Should().NotBeNull();
			initial.Children.Should().NotBeNullOrEmpty();
		    intrans.Children.Should().NotBeNullOrEmpty();
		    outside.Children.Should().NotBeNullOrEmpty();
			initial.Children.Count().Should().Be(intrans.Children.Count() - 4);
		    initial.Children.Count().Should().Be(outside.Children.Count());
		}

		[Fact]
		public void Get_Should_Load_Object_By_Identity()
	    {
		    ParentModel parent = _repo.GetOnlyParentByName("crm");
		    parent = _repo.GetParentById(parent.Id);
			ClientRedirectUri uri = _repo.Get<ClientRedirectUri>(parent.Children.First().ClientRedirectUriId);

		    parent.Should().NotBeNull();
		    parent.Children.Should().NotBeNullOrEmpty();
		    uri.Should().NotBeNull();
		}

		[Fact]
		public async Task Get_Should_Load_Object_By_Identity_Async()
	    {
		    ParentModel parent = await _repo.GetOnlyParentByNameAsync("crm");
		    parent = _repo.GetParentById(parent.Id);
		    ClientRedirectUri uri = await _repo.GetAsync<ClientRedirectUri>(parent.Children.First().ClientRedirectUriId);

		    parent.Should().NotBeNull();
		    parent.Children.Should().NotBeNullOrEmpty();
		    uri.Should().NotBeNull();
	    }

	    [Fact]
	    public void UseConnection_Should_Allow_Multiple_Operations_In_Delegate()
	    {
		    (IEnumerable<ParentModel> list, ParentModel single, ParentModel full) result = _repo.DbOperation("crm");

		    result.list.Count().Should().BeGreaterThan(0);
		    result.single.Should().NotBeNull();
		    result.full.Should().NotBeNull();
		    result.single.Name.Should().Be(result.full.Name);
		    result.full.Children.Count().Should().BeGreaterThan(0);
		}

	    [Fact]
	    public async Task AsyncUseConnection_Should_Allow_Multiple_Operations_In_Delegate()
	    {
		    (IEnumerable<ParentModel> list, ParentModel single, ParentModel full) result = await _repo.DbOperationAsync("crm");

		    result.list.Count().Should().BeGreaterThan(0);
		    result.single.Should().NotBeNull();
		    result.full.Should().NotBeNull();
		    result.single.Name.Should().Be(result.full.Name);
		    result.full.Children.Count().Should().BeGreaterThan(0);
	    }

	    [Fact]
	    public async Task Multiple_Get_Operations_Should_All_Work()
	    {
		    ParentModel parent = await _repo.GetOnlyParentByNameAsync("crm");
			List<Task<ParentModel>> gets = new List<Task<ParentModel>>();

		    for (int i = 0; i < 10; i++)
		    {
				gets.Add(_repo.GetParentByIdAsync(parent.Id));
			}

		    await Task.WhenAll(gets);

		    foreach (Task<ParentModel> task in gets)
		    {
			    task.Result.Should().NotBeNull();
		    }
	    }
	}
}
