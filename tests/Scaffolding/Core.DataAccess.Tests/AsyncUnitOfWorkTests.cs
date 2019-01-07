using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Scaffolding.Core.DataAccess.Tests
{
	public class AsyncUnitOfWorkTests
	{
		private readonly Mock<IDbConnectionProvider> _provider;
		private readonly Mock<IDbConnection> _connection;
		private readonly Mock<IDbTransaction> _transaction;
		private readonly List<IConnectionRepo> _repos;
		private AsyncUnitOfWork _unitOfWork;
		private int _transactionCount = 0;
		private static readonly Dictionary<string, int> _statements = new Dictionary<string, int>
		{
			{ "insert", 5 },
			{ "update", 10 },
			{ "delete", 15 },
			{ "select", 20 }
		};

		public AsyncUnitOfWorkTests()
		{
			_unitOfWork = null;
			_provider = new Mock<IDbConnectionProvider>();
			_connection = new Mock<IDbConnection>();
			_transaction = new Mock<IDbTransaction>();
			_repos = new List<IConnectionRepo>();

			_provider.Setup(p => p.GetConnection()).Returns(_connection.Object);
			_connection.Setup(c => c.BeginTransaction()).Returns(_transaction.Object).Callback(() => { _transactionCount++; });
		}

		private Mock<IConnectionRepo> BuildRepo()
		{
			Mock<IConnectionRepo> repo = new Mock<IConnectionRepo>();
			repo.SetupAllProperties();

			foreach (string key in _statements.Keys)
			{
				repo.Setup(r => r.ExecuteAsync(key, null, null)).Returns(Task.FromResult(_statements[key]));
			}

			return repo;
		}

		[Fact]
		public void Null_Repos_Does_Not_Blow_Up()
		{
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, null);

			_unitOfWork.InTransaction.Should().BeFalse();
			_unitOfWork.NumberOfRepos.Should().Be(0);
			_transactionCount.Should().Be(0);
		}

		[Fact]
		public void SingleRepo_Does_Not_Begin_Transaction()
		{
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);

			_unitOfWork.InTransaction.Should().BeFalse();
			_transactionCount.Should().Be(0);
		}

		[Fact]
		public void MultiRepo_All_Share_Unit_Of_Work()
		{
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);

			foreach (IConnectionRepo repo in _repos)
			{
				repo.AsyncUnitOfWork.Should().Be(_unitOfWork);
			}
		}

		[Fact]
		public async Task BeginTransaction_Initializes_Transaction_When_One_Not_Present()
		{
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);

			await _unitOfWork.BeginTransactionAsync();

			_unitOfWork.InTransaction.Should().BeTrue();
			_transactionCount.Should().Be(1);
		}

		[Fact]
		public async Task BeginTransaction_Does_Not_ReInitialize_Transaction()
		{
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);

			await _unitOfWork.BeginTransactionAsync();

			_unitOfWork.InTransaction.Should().BeTrue();
			_transactionCount.Should().Be(1);
		}

		[Fact]
		public void CommitTransaction_Works_When_No_Transaction_Present()
		{
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);
			_unitOfWork.CommitTransaction();

			_unitOfWork.InTransaction.Should().BeFalse();
			_transactionCount.Should().Be(0);
		}

		[Fact]
		public async Task CommitTransaction_Clears_Transaction_When_Executed()
		{
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);
			await _unitOfWork.BeginTransactionAsync();
			_unitOfWork.InTransaction.Should().BeTrue();
			_unitOfWork.CommitTransaction();
			_unitOfWork.InTransaction.Should().BeFalse();
			_transactionCount.Should().Be(1);
		}

		[Fact]
		public void CommitTransaction_Clears_Repo_Units_Of_Work()
		{
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);

			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);
			_unitOfWork.CommitTransaction();

			foreach (IConnectionRepo repo in _repos)
			{
				repo.UnitOfWork.Should().BeNull();
			}
		}

		[Fact]
		public void RollbackTransaction_Works_When_No_Transaction_Present()
		{
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);
			_unitOfWork.RollbackTransaction();

			_unitOfWork.InTransaction.Should().BeFalse();
			_transactionCount.Should().Be(0);
		}

		[Fact]
		public async Task RollbackTransaction_Clears_Transaction_When_Executed()
		{
			_repos.Add(BuildRepo().Object);
			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);
			await _unitOfWork.BeginTransactionAsync();
			_unitOfWork.InTransaction.Should().BeTrue();
			_unitOfWork.RollbackTransaction();
			_unitOfWork.InTransaction.Should().BeFalse();
			_transactionCount.Should().Be(1);
		}

		[Fact]
		public void RollbackTransaction_Clears_Repo_Units_Of_Work()
		{
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);

			_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos);
			_unitOfWork.RollbackTransaction();

			foreach (IConnectionRepo repo in _repos)
			{
				repo.UnitOfWork.Should().BeNull();
			}
		}

		[Fact]
		public async void MultiRepo_With_Using_Statement_Happy_Path()
		{
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);
			_repos.Add(BuildRepo().Object);

			// initialize multi-operation Unit Of Work (UOW)
			using (_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos))
			{
				await _unitOfWork.BeginTransactionAsync();

				for (int idx = 0; idx < _statements.Keys.Count; idx++)
				{
					string key = _statements.Keys.ToList()[idx];
					int result = await _repos[idx].ExecuteAsync(key);

					// did correct statement get called?
					result.Should().Be(_statements[key]);

					// is call considered in transaction?
					_unitOfWork.InTransaction.Should().BeTrue();

					// are all repos sharing same UOW
					_repos[idx].AsyncUnitOfWork.Should().Be(_unitOfWork);

					// sync uow is null
					_repos[idx].UnitOfWork.Should().BeNull();

					// is UOW disposed?
					_unitOfWork.Disposed.Should().BeFalse();
				}

				// commit multi-operation work
				_unitOfWork.CommitTransaction();
			}

			// Transaction over?
			_unitOfWork.InTransaction.Should().BeFalse();

			// Transaction count correct?
			_transactionCount.Should().Be(1);

			// Object disposed?
			_unitOfWork.Disposed.Should().BeTrue();

			foreach (IConnectionRepo repo in _repos)
			{
				// repo's cleaned up
				repo.UnitOfWork.Should().BeNull();
			}
		}

		[Fact]
		public async void MultiRepo_With_Using_Statement_UnHappy_Path()
		{
			Mock<IConnectionRepo> forError = BuildRepo();
			forError.Setup(r => r.ExecuteAsync("error", null, null)).Throws(new Exception("boom"));

			_repos.Add(BuildRepo().Object);
			_repos.Add(forError.Object);

			// initialize multi-operation Unit Of Work (UOW)
			using (_unitOfWork = new AsyncUnitOfWork(_provider.Object, _repos))
			{
				try
				{
					await _unitOfWork.BeginTransactionAsync();
					await _repos[0].ExecuteAsync("insert");
					_unitOfWork.InTransaction.Should().BeTrue();
					await _repos[1].ExecuteAsync("error");
					_unitOfWork.Disposed.Should().BeFalse();
				}
				catch (Exception e)
				{
					_unitOfWork.RollbackTransaction();
				}
			}

			// Transaction over?
			_unitOfWork.InTransaction.Should().BeFalse();

			// Transaction count correct?
			_transactionCount.Should().Be(1);

			// Object disposed?
			_unitOfWork.Disposed.Should().BeTrue();

			foreach (IConnectionRepo repo in _repos)
			{
				// repo's cleaned up
				repo.AsyncUnitOfWork.Should().BeNull();
			}
		}
	}
}