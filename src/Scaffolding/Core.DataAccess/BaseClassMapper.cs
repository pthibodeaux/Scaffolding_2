using DapperExtensions.Mapper;

namespace Scaffolding.Core.DataAccess
{
	public abstract class BaseClassMapper<TTable> : ClassMapper<TTable> where TTable : class
	{
		protected BaseClassMapper()
		{
			Initialize();
		}

		private void Initialize()
		{
			CustomMappings();
			AutoMap();
		}

		public abstract void CustomMappings();

		public override void Table(string tableName)
		{
			base.Table($"{tableName}s");
		}
	}
}
