using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;
using Tgs.GameInterface.V8Poker.Core.Models;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class FundInGetServiceUnitTest
{
    [Fact]
    public async Task Normal_ProcessAsync()
    {
        var query = new FundInQuery(
            ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033",
            PlayerCode: "4ue0uj8cwn2v",
            OrderId: "71033202310120338068944ue0uj8cwn2v",
            Amount: 1.0m);

        var fakeClient = Substitute.For<IGameApiClient>();

        _ = fakeClient
            .WithdrawAsync(
            Arg.Is(query.ApiUrl),
            Arg.Is(query.Deskey),
            Arg.Is(query.MD5key),
            Arg.Is(query.AgentId),
            Arg.Is(query.PlayerCode),
            Arg.Is(query.OrderId),
            Arg.Is(query.Amount),
            Arg.Any<CancellationToken>())
            .Returns(new ValueTask<WithdrawResp>(
                new WithdrawResp(Resp: "{\"m\":\"/channelHandle\",\"s\":103,\"d\":{\"code\":0,\"money\":\"0.00\"}}")));

        var service = new FundInGetService(
            fakeClient);

        var actual = await service.GetAsync(query: query);

        Assert.Equal("{\"m\":\"/channelHandle\",\"s\":103,\"d\":{\"code\":0,\"money\":\"0.00\"}}", actual.Resp);
    }
}
