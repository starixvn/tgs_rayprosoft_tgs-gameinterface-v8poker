using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class PlayerLogoutCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_ProcessAsync()
    {
        var command = new PlayerLogoutCommand(
            ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033",
            PlayerCode: "4ue0uj8cwn2v");

        var fakeClient = Substitute.For<IGameApiClient>();

        var service = new PlayerLogoutCommandService(
            fakeClient);

        await service.ExecuteAsync(
            command: command);

        _ = fakeClient
            .Received(1)
            .LogoutAsync(
            Arg.Is(command.ApiUrl),
            Arg.Is(command.Deskey),
            Arg.Is(command.MD5key),
            Arg.Is(command.AgentId),
            Arg.Is(command.PlayerCode),
            Arg.Any<CancellationToken>());
    }
}
