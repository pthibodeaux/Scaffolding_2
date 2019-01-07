using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Scaffolding.Core.DI.Repositories;

namespace Scaffolding.Core.DataAccess
{
	public abstract class BaseRepository : IConnectionRepo
	{
		public const int DEADLOCK_ERROR_NBR = 1205;

		protected static readonly ConcurrentDictionary<string, string> _statements = new ConcurrentDictionary<string, string>();
		private readonly IUnitOfWorkProvider _unitOfWorkProvider;

		private int _tryCount = 0;
		private readonly Random _random = new Random();

		protected BaseRepository(IUnitOfWorkProvider unitOfWorkProvider)
		{
			SqlMapper.AddTypeHandler(new DateTimeHandler());
			_unitOfWorkProvider = unitOfWorkProvider;
		}

		public IConnection UnitOfWork { get; set; }
		
	    public IAsyncConnection AsyncUnitOfWork { get; set; }

	    public virtual IUnitOfWork CreateUnitOfWork(IEnumerable<IBaseRepository> repos)
	    {
		    return CreateDbConnection(repos);
		}

		public virtual IUnitOfWork CreateUnitOfWork(IBaseRepository repo)
		{
			return CreateDbConnection(new [] { repo });
		}

		public virtual IUnitOfWork CreateUnitOfWork()
		{
			return CreateDbConnection(new[] { this });
		}

		protected virtual IConnection CreateDbConnection(IEnumerable<IBaseRepository> repos = null)
		{
			IConnection uow = _unitOfWorkProvider.GetUnitOfWork(repos);
			
			if (uow.NumberOfRepos > 1)
			{
				uow.BeginTransaction();
			}

			return uow;
		}

	    public virtual async Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork(IEnumerable<IBaseRepository> repos)
	    {
		    return await CreateAsyncDbConnection(repos);
		}

		public virtual async Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork(IBaseRepository repo)
		{
			return await CreateAsyncDbConnection(new[] { repo });
		}

		public virtual async Task<IAsyncUnitOfWork> CreateAsyncUnitOfWork()
		{
			return await CreateAsyncDbConnection(new[] { this });
		}

		protected virtual async Task<IAsyncConnection> CreateAsyncDbConnection(IEnumerable<IBaseRepository> repos = null)
		{
			IAsyncConnection uow = _unitOfWorkProvider.GetAsyncUnitOfWork(repos);

			if (uow.NumberOfRepos > 1)
			{
				await uow.BeginTransactionAsync();
			}

			return uow;
		}

		public string GetSql(string resource)
		{
			return _statements.GetOrAdd(resource, GetSqlFromResource);
		}

		public string GetSqlFromResource(string resource)
		{
			using (Stream stream = GetType().Assembly.GetManifestResourceStream(resource))
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public List<T> Query<T>(string sql, object arg = null)
		{
			return DoWork(uow => uow.Query<T>(sql, arg));
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object arg = null)
	    {
		    return await DoWorkAsync(async uow => await uow.QueryAsync<T>(sql, arg));
	    }

		public T QuerySingle<T>(string sql, object arg = null)
		{
			return DoWork(uow => uow.QuerySingle<T>(sql, arg));
		}

		public async Task<T> QuerySingleAsync<T>(string sql, object arg = null)
	    {
		    return await DoWorkAsync(async uow => await uow.QuerySingleAsync<T>(sql, arg));
	    }

		public int Execute(string sql, object arg = null)
		{
			return DoWork(uow => uow.Execute(sql, arg));
		}

		public async Task<int> ExecuteAsync(string sql, object arg = null, CommandType? commandType = null)
	    {
		    return await DoWorkAsync(async uow => await uow.ExecuteAsync(sql, arg, commandType));
		}

		public T Get<T>(object id) where T : class
		{
			return DoWork(uow => uow.Get<T>(id));
		}

		public async Task<T> GetAsync<T>(object id) where T : class
	    {
		    return await DoWorkAsync(async uow => await uow.GetAsync<T>(id));
	    }

		public object Insert<T>(T obj) where T : class
		{
			return DoWork(uow => uow.Insert(obj));
		}

	    public async Task<object> InsertAsync<T>(T obj) where T : class
	    {
		    return await DoWorkAsync(async uow => await uow.InsertAsync(obj));
	    }

		public bool Update<T>(T obj) where T : class
		{
			return DoWork(uow => uow.Update(obj));
		}

	    public async Task<bool> UpdateAsync<T>(T obj) where T : class
	    {
		    return await DoWorkAsync(async uow => await uow.UpdateAsync(obj));
	    }

		public bool Delete<T>(T obj) where T : class
		{
			return DoWork(uow => uow.Delete(obj));
		}

	    public async Task<bool> DeleteAsync<T>(T obj) where T : class
	    {
			return await DoWorkAsync(async uow => await uow.DeleteAsync(obj));
		}

		protected void UseConnection(Action<IDbConnection, IDbTransaction> work)
		{
			DoWork(uow =>
			{
				uow.UseConnection(work);
				return true;
			});
		}

		protected async Task UseConnectionAsync(Func<IDbConnection, IDbTransaction, Task> work)
		{
			await DoWorkAsync(async uow =>
			{
				await uow.UseConnectionAsync(work);
				return true;
			});
		}

		protected virtual T DoWork<T>(Func<IConnection, T> work)
		{
			try
			{
				T result;

				if (UnitOfWork != null && !UnitOfWork.Disposed)
				{
					if (!UnitOfWork.IsOpen) 
					{
						throw new Exception("Connection not open.  Please call BeginTransaction() to open the connection.");
					}

					result = work(UnitOfWork);
				}
				else
				{
					// not passing in any repos to the UOW creation allows multiple repository operations 
					// to each independantly do their DB work on separate connections
					using (IConnection tempUnitOfWork = CreateDbConnection())
					{
						// since there isn't a transaction, the UOW needs to be opened.
						tempUnitOfWork.Open();
						result = work(tempUnitOfWork);
					}
				}

				_tryCount = 0;
				return result;
			}
			catch (SqlException ex) when (ex.Number == DEADLOCK_ERROR_NBR)
			{
				if (_tryCount < 3)
				{
					_tryCount++;
					Thread.Sleep(_random.Next(100, 200));
					return DoWork(work);
				}

				throw;
			}
		}

	    protected virtual async Task<T> DoWorkAsync<T>(Func<IAsyncConnection, Task<T>> work)
	    {
		    try
		    {
			    T result;

			    if (AsyncUnitOfWork != null && !AsyncUnitOfWork.Disposed)
			    {
				    if (!AsyncUnitOfWork.IsOpen) 
					{
						throw new Exception("Connection not open.  Please call BeginTransaction() to open the connection.");
					}

					result = await work(AsyncUnitOfWork);
			    }
			    else
			    {
					// not passing in any repos to the UOW creation allows multiple repository operations 
					// to each independantly do their DB work on separate connections
				    using (IAsyncConnection tempUnitOfWork = await CreateAsyncDbConnection())
				    {
					    // since there isn't a transaction, the UOW needs to be opened.
					    await tempUnitOfWork.OpenAsync();
					    result = await work(tempUnitOfWork);
				    }
				}

			    _tryCount = 0;
				return result;
		    }
		    catch (SqlException ex) when (ex.Number == DEADLOCK_ERROR_NBR)
		    {
			    if (_tryCount < 3)
			    {
				    _tryCount++;
				    Thread.Sleep(_random.Next(100, 200));
					return await DoWorkAsync(work);
			    }

			    throw;
		    }
		}
	}
}