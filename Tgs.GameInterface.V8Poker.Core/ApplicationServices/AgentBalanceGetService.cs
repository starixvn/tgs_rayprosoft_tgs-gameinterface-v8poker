using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

internal class AgentBalanceGetService : IGetService<AgentBalanceQuery, AgentBalanceResult>
{
    private readonly IGameApiClient _client;

    public AgentBalanceGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<AgentBalanceResult> GetAsync(AgentBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var credit = await _client.AgentBalanceAsync(
            apiUrl: query.ApiUrl,
            deskey: query.Deskey,
            md5key: query.MD5key,
            agentId: query.AgentId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new AgentBalanceResult(credit);
    }
}
