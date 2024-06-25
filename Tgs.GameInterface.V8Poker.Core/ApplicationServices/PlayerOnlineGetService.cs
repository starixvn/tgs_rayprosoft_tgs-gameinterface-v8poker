using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

public class PlayerOnlineGetService(
	IGameApiClient client) : IGetService<PlayerOnlineQuery, bool>
{
	public ValueTask<bool> GetAsync(PlayerOnlineQuery query, CancellationToken cancellationToken = default)
		=> client.PlayerOnlineAsync(
			apiUrl: query.ApiUrl,
			deskey: query.Deskey,
			md5key: query.MD5key,
			agentId: query.AgentId,
			account: query.PlayerCode,
			cancellationToken: cancellationToken);
}
