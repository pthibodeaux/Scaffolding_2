using DapperExtensions.Mapper;

namespace Scaffolding.Core.DataAccess.Tests.Integration
{
	public class ClientRedirectUriMapper : BaseClassMapper<ClientRedirectUri>
	{
		public override void CustomMappings()
		{
			Map(i => i.ClientRedirectUriId).Key(KeyType.Identity);
		}
	}
}
