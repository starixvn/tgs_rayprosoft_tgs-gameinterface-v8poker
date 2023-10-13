using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

internal class PlayerLogoutCommandService : ICommandService<PlayerLogoutCommand>
{
    private readonly IGameApiClient _client;

    public PlayerLogoutCommandService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(
        PlayerLogoutCommand command,
        CancellationToken cancellationToken = default)
        => _client
            .LogoutAsync(
            apiUrl: command.ApiUrl,
            deskey: command.Deskey,
            md5key: command.MD5key,
            agentId: command.AgentId,
            account: command.PlayerCode,
            cancellationToken: cancellationToken);
}
