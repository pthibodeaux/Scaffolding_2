using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Scaffolding.Core.DataAccess.Tests.Fixtures;
using Scaffolding.Core.DI.Repositories;
using Xunit;

namespace Scaffolding.Core.DataAccess.Tests
{
    public class BaseRepositoryTests
    {
	    private readonly Mock<IUnitOfWorkProvider> _provider;
	    private readonly Mock<IConnection> _unitOfWork;
	    private readonly Mock<IAsyncConnection> _asyncUnitOfWork;
		private readonly FakeRepository _repo;
	    private bool _uowDisposed = false;
	    private bool _inTransaction = false;
	    private int _transactionCount = 0;

		private readonly string[] statements = {"some-string-five", "some-string-ten", "some-string-fifteen"};

		public BaseRepositoryTests()
		{
			_provider = new Mock<IUnitOfWorkProvider>();
			_unitOfWork = new Mock<IConnection>();
			_asyncUnitOfWork = new Mock<IAsyncConnection>();

			_provider.Setup(p => p.GetUnitOfWork(It.IsAny<List<IBaseRepository>>())).Returns(_unitOfWork.Object);
			_provider.Setup(p => p.GetUnitOfWork(It.IsAny<List<IConnectionRepo>>())).Returns(_unitOfWork.Object);
			_provider.Setup(p => p.GetAsyncUnitOfWork(It.IsAny<List<IBaseRepository>>())).Returns(_asyncUnitOfWork.Object);
			_provider.Setup(p => p.GetAsyncUnitOfWork(It.IsAny<List<IConnectionRepo>>())).Returns(_asyncUnitOfWork.Object);
			_unitOfWork.Setup(uow => uow.Execute(statements[0], null)).Returns(5);
			_unitOfWork.Setup(uow => uow.Execute(statements[1], null)).Returns(10);
			_unitOfWork.Setup(uow => uow.Execute(statements[2], null)).Returns(15);
			_unitOfWork.Setup(uow => uow.BeginTransaction()).Callback(() =>
			{
				_inTransaction = true;
				_transactionCount++;
			});
			_unitOfWork.Setup(uow => uow.Dispose()).Callback(() =>
			{
				_uowDisposed = true;
			});
			_asyncUnitOfWork.Setup(uow => uow.BeginTransactionAsync()).Returns(Task.CompletedTask).Callback(() =>
			{
				_inTransaction = true;
				_transactionCount++;
			});
			_asyncUnitOfWork.Setup(uow => uow.Dispose()).Callback(() =>
			{
				_uowDisposed = true;
			});
			_repo = new FakeRepository(_provider.Object);
		}

	    [Fact]
	    public void GetSqlFromEmbeddedResource_Should_Return_Contents_Of_Sql_File()
	    {
		    string sql = _repo.GetSqlFromResource("Scaffolding.Core.DataAccess.Tests.Fixtures.TestSql.sql");
			
		    sql.Should().NotBeNullOrEmpty();
		}

	    [Fact]
	    public void GetSql_Should_Cache_Statement_In_Dictionary()
	    {
		    _repo.Statements.Clear();
		    string sql = _repo.GetSql("Scaffolding.Core.DataAccess.Tests.Fixtures.TestSql.sql");

		    sql.Should().NotBeNullOrEmpty();
		    sql.Should().Be(_repo.GetSql("Scaffolding.Core.DataAccess.Tests.Fixtures.TestSql.sql"));
			_repo.Statements.Count.Should().Be(1);
	    }

	    [Fact]
	    public void Execute_Should_Create_Its_Own_UnitOfWork()
	    {
		    int result = _repo.Execute("some-string-five");

			result.Should().Be(5);
			_repo.UnitOfWork.Should().BeNull();
		    _uowDisposed.Should().BeTrue();
		}

	    [Fact]
	    public void MultiRepo_Does_Begin_Transaction()
	    {
			List<IConnectionRepo> toTest = new List<IConnectionRepo>();

		    foreach (string statement in statements)
		    {
			    toTest.Add(new FakeRepository(_provider.Object));
		    }
			
		    _unitOfWork.Setup(uow => uow.NumberOfRepos).Returns(toTest.Count);
			toTest[1].CreateUnitOfWork(toTest);

		    foreach (IConnectionRepo repo in toTest)
		    {
			    _inTransaction.Should().BeTrue();
			    _transactionCount.Should().Be(1);
		    }
		}

	    [Fact]
	    public async void Async_MultiRepo_Does_Begin_Transaction()
	    {
		    List<IConnectionRepo> toTest = new List<IConnectionRepo>();

		    foreach (string statement in statements)
		    {
			    toTest.Add(new FakeRepository(_provider.Object));
		    }

		    _asyncUnitOfWork.Setup(uow => uow.NumberOfRepos).Returns(toTest.Count);
		    await toTest[1].CreateAsyncUnitOfWork(toTest);

		    foreach (IConnectionRepo repo in toTest)
		    {
			    _inTransaction.Should().BeTrue();
			    _transactionCount.Should().Be(1);
		    }
	    }

	    [Fact]
	    public async void Async_Unopened_UnitOfWork_Throws_Exception()
	    {
		    IConnectionRepo repo = new FakeRepository(_provider.Object);
		    repo.AsyncUnitOfWork = _asyncUnitOfWork.Object;

		    _asyncUnitOfWork.Setup(uow => uow.NumberOfRepos).Returns(1);

		    await Assert.ThrowsAsync<Exception>(async () =>  await repo.ExecuteAsync("sql"));
	    }

		[Fact]
	    public void Multiple_Repos_Share_Same_Unit_Of_Work()
	    {
			Dictionary<string, IConnectionRepo> toTest = new Dictionary<string, IConnectionRepo>();

		    foreach (string statement in statements)
		    {
			    toTest.Add(statement, new FakeRepository(_provider.Object));
			    toTest[statement].UnitOfWork = _unitOfWork.Object;
			}

		    _unitOfWork.Setup(uow => uow.NumberOfRepos).Returns(toTest.Count);
		    _unitOfWork.Setup(uow => uow.IsOpen).Returns(true);

			foreach (string statement in statements)
		    {
			    int result = toTest[statement].Execute(statement);

				toTest[statement].UnitOfWork.Should().NotBeNull();
				(result % 5 == 0).Should().BeTrue();
			    _uowDisposed.Should().BeFalse();
			}
		}

	    [Fact]
	    public void Unopened_UnitOfWork_Throws_Exception()
	    {
		    IConnectionRepo repo = new FakeRepository(_provider.Object);
			repo.UnitOfWork = _unitOfWork.Object;

		    _unitOfWork.Setup(uow => uow.NumberOfRepos).Returns(1);

		    Assert.Throws<Exception>(() =>  repo.Execute("sql"));
	    }

	    [Fact]
	    public void DeadLock_Exception_Should_Be_Retried_Once()
	    {
		    int counter = 0;
			SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
				    .WithErrorMessage("Fake Deadlock")
				    .Build();

			_unitOfWork.Setup(uow => uow.Execute("deadlock", null)).Returns(() =>
		    {
			    if (counter == 0)
			    {
				    counter++;
				    throw ex;
				}

			    return counter;
		    });

		    int result = _repo.Execute("deadlock");

		    result.Should().Be(1);
		}

	    [Fact]
	    public void DeadLock_Exception_Should_Be_Retried_Twice()
	    {
		    int counter = 0;
		    SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
			    .WithErrorMessage("Fake Deadlock")
			    .Build();

		    _unitOfWork.Setup(uow => uow.Execute("deadlock", null)).Returns(() =>
		    {
			    if (counter < 2)
			    {
				    counter++;
				    throw ex;
			    }

			    return counter;
		    });

		    int result = _repo.Execute("deadlock");

		    result.Should().Be(2);
		}

	    [Fact]
	    public void DeadLock_Exception_Should_Be_Retried_Three()
	    {
		    int counter = 0;
		    SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
			    .WithErrorMessage("Fake Deadlock")
			    .Build();

		    _unitOfWork.Setup(uow => uow.Execute("deadlock", null)).Returns(() =>
		    {
			    if (counter < 3)
			    {
				    counter++;
				    throw ex;
			    }

			    return counter;
		    });

		    int result = _repo.Execute("deadlock");

		    result.Should().Be(3);
		}

	    [Fact]
	    public void DeadLock_Exception_Should_Be_Stop_After_Three()
	    {
		    int counter = 0;
		    SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
			    .WithErrorMessage("Fake Deadlock")
			    .Build();

		    _unitOfWork.Setup(uow => uow.Execute("deadlock", null)).Returns(() =>
		    {
				// should fail before this, but preventing infinate loop
			    if (counter < 10)
			    {
				    counter++;
				    throw ex;
			    }

			    return counter;
		    });

		    Assert.Throws<SqlException>(() => _repo.Execute("deadlock")).Number.Should().Be(BaseRepository.DEADLOCK_ERROR_NBR);
		    counter.Should().Be(4);
		}

	    [Fact]
	    public void Non_DeadLock_Sql_Error_Exception_Should_Not_Retry()
	    {
		    int counter = 0;
		    SqlException ex = new SqlExceptionBuilder().WithErrorNumber(8675309)
			    .WithErrorMessage("Fake Deadlock")
			    .Build();

		    _unitOfWork.Setup(uow => uow.Execute("deadlock", null)).Returns(() =>
		    {
			    // should fail before this, but preventing infinate loop
			    if (counter < 10)
			    {
				    counter++;
				    throw ex;
			    }

			    return counter;
		    });

		    Assert.Throws<SqlException>(() => _repo.Execute("deadlock")).Number.Should().Be(8675309);
		    counter.Should().Be(1);
		}

	    [Fact]
	    public void Non_DeadLock_Exception_Should_Not_Retry()
	    {
		    int counter = 0;

			_unitOfWork.Setup(uow => uow.Execute("deadlock", null)).Returns(() =>
			{
			    // should fail before this, but preventing infinate loop
			    if (counter < 10)
			    {
				    counter++;
				    throw new InvalidOperationException("boom");
			    }

			    return counter;
		    });

		    Assert.Throws<InvalidOperationException>(() => _repo.Execute("deadlock"));
		    counter.Should().Be(1);
	    }

		[Fact]
		public async void DeadLock_Async_Exception_Should_Be_Retried_Once()
		{
			int counter = 0;
			SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
					.WithErrorMessage("Fake Deadlock")
					.Build();

			_asyncUnitOfWork.Setup(uow => uow.ExecuteAsync("deadlock", null, null)).Returns(async () =>
			{
				if (counter == 0)
				{
					counter++;
					throw ex;
				}

				return counter;
			});

			int result = await _repo.ExecuteAsync("deadlock");

			result.Should().Be(1);
		}

		[Fact]
		public async void DeadLock_Async_Exception_Should_Be_Retried_Twice()
		{
			int counter = 0;
			SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
				.WithErrorMessage("Fake Deadlock")
				.Build();

			_asyncUnitOfWork.Setup(uow => uow.ExecuteAsync("deadlock", null, null)).Returns(async () =>
			{
				if (counter < 2)
				{
					counter++;
					throw ex;
				}

				return counter;
			});

			int result = await _repo.ExecuteAsync("deadlock");

			result.Should().Be(2);
		}

		[Fact]
		public async void DeadLock_Async_Exception_Should_Be_Retried_Three()
		{
			int counter = 0;
			SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
				.WithErrorMessage("Fake Deadlock")
				.Build();

			_asyncUnitOfWork.Setup(uow => uow.ExecuteAsync("deadlock", null, null)).Returns(async () =>
			{
				if (counter < 3)
				{
					counter++;
					throw ex;
				}

				return counter;
			});

			int result = await _repo.ExecuteAsync("deadlock");

			result.Should().Be(3);
		}

		[Fact]
		public async void DeadLock_Async_Exception_Should_Be_Stop_After_Three()
		{
			int counter = 0;
			SqlException ex = new SqlExceptionBuilder().WithErrorNumber(BaseRepository.DEADLOCK_ERROR_NBR)
				.WithErrorMessage("Fake Deadlock")
				.Build();

			_asyncUnitOfWork.Setup(uow => uow.ExecuteAsync("deadlock", null, null)).Returns(async () =>
			{
				// should fail before this, but preventing infinate loop
				if (counter < 10)
				{
					counter++;
					throw ex;
				}

				return counter;
			});

			await Assert.ThrowsAsync<SqlException>(async () => await _repo.ExecuteAsync("deadlock"));
			counter.Should().Be(4);
		}

		[Fact]
		public async void Non_DeadLock_Sql_Error_Async_Exception_Should_Not_Retry()
		{
			int counter = 0;
			SqlException ex = new SqlExceptionBuilder().WithErrorNumber(8675309)
				.WithErrorMessage("Fake Error")
				.Build();

			_asyncUnitOfWork.Setup(uow => uow.ExecuteAsync("deadlock", null, null)).Returns(async () =>
			{
				// should fail before this, but preventing infinate loop
				if (counter < 10)
				{
					counter++;
					throw ex;
				}

				return counter;
			});

			await Assert.ThrowsAsync<SqlException>(async () => await _repo.ExecuteAsync("deadlock"));
			counter.Should().Be(1);
		}

	    [Fact]
	    public async void Non_DeadLock_Async_Exception_Should_Not_Retry()
	    {
		    int counter = 0;

		    _asyncUnitOfWork.Setup(uow => uow.ExecuteAsync("deadlock", null, null)).Returns(async () =>
		    {
			    // should fail before this, but preventing infinate loop
			    if (counter < 10)
			    {
				    counter++;
				    throw new InvalidOperationException("boom");
			    }

			    return counter;
		    });

		    await Assert.ThrowsAsync<InvalidOperationException>(async () => await _repo.ExecuteAsync("deadlock"));
		    counter.Should().Be(1);
	    }
	}
}
