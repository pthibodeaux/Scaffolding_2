using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Scaffolding.Core.DataAccess.Tests.Fixtures;
using Scaffolding.Core.DI.Repositories;
using Xunit;

namespace Scaffolding.Core.DataAccess.Tests
{
    public class BaseRepositoryExtentionsTests
    {
	    private readonly Mock<IUnitOfWorkProvider> _provider;
	    private readonly Mock<IConnection> _mockUow = new Mock<IConnection>();
	    private readonly Mock<IAsyncConnection> _mockAuow = new Mock<IAsyncConnection>();

		public BaseRepositoryExtentionsTests()
	    {
			_mockUow = new Mock<IConnection>();
			_mockAuow = new Mock<IAsyncConnection>();
			_provider = new Mock<IUnitOfWorkProvider>();
		    _provider.Setup(p => p.GetUnitOfWork(It.IsAny<IEnumerable<IBaseRepository>>())).Returns(_mockUow.Object);
		    _provider.Setup(p => p.GetAsyncUnitOfWork(It.IsAny<IEnumerable<IBaseRepository>>())).Returns(_mockAuow.Object);
		}

	    [Fact]
	    public void GetUnitOfWork_Should_Return_From_List()
	    {
		    IEnumerable<IBaseRepository> repos = new List<IBaseRepository>
		    {
			    new FakeRepository(_provider.Object),
			    new FakeRepository(_provider.Object),
			    new FakeRepository(_provider.Object),
			    new FakeRepository(_provider.Object)
			};

		    IUnitOfWork uow = repos.CreateUnitOfWork();

		    uow.Should().Be(_mockUow.Object);
		}

	    [Fact]
	    public void GetUnitOfWork_Should_Blow_When_List_Empty()
	    {
		    IEnumerable<IBaseRepository> repos = new List<IBaseRepository> { };

		    Assert.Throws<NullReferenceException>(() => repos.CreateUnitOfWork());
		}

	    [Fact]
	    public async void GetAsyncUnitOfWork_Should_Return_From_List()
	    {
		    IEnumerable<IBaseRepository> repos = new List<IBaseRepository>
		    {
			    new FakeRepository(_provider.Object),
			    new FakeRepository(_provider.Object),
			    new FakeRepository(_provider.Object),
			    new FakeRepository(_provider.Object)
		    };

		    IAsyncUnitOfWork uow = await repos.CreateAsyncUnitOfWork();

		    uow.Should().Be(_mockAuow.Object);
	    }

	    [Fact]
	    public async void GetAsyncUnitOfWork_Should_Blow_When_List_Empty()
	    {
		    IEnumerable<IBaseRepository> repos = new List<IBaseRepository> { };

		    await Assert.ThrowsAsync<NullReferenceException>(async () => await repos.CreateAsyncUnitOfWork());
	    }
	}
}
