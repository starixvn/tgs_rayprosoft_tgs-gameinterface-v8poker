using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

internal class PlayerLogoutCommandService(
    IGameApiClient client) : ICommandService<PlayerLogoutCommand>
{
    public ValueTask ExecuteAsync(
        PlayerLogoutCommand command,
        CancellationToken cancellationToken = default)
        => client
            .LogoutAsync(
            apiUrl: command.ApiUrl,
            deskey: command.Deskey,
            md5key: command.MD5key,
            agentId: command.AgentId,
            account: command.PlayerCode,
            cancellationToken: cancellationToken);
}
