using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

internal class AgentBalanceGetService(IGameApiClient client) : IGetService<AgentBalanceQuery, AgentBalanceResult>
{
	public async ValueTask<AgentBalanceResult> GetAsync(AgentBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var credit = await client.AgentBalanceAsync(
			apiUrl: query.ApiUrl,
			deskey: query.Deskey,
			md5key: query.MD5key,
			agentId: query.AgentId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new AgentBalanceResult(credit);
	}
}
