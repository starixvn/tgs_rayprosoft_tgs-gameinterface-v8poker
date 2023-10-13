using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class AgentBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Normal_ProcessAsync()
    {
        var query = new AgentBalanceQuery(
            ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033");

        var fakeClient = Substitute.For<IGameApiClient>();

        var credit = 10000000.00m;
        _ = fakeClient
            .AgentBalanceAsync(
            Arg.Is(query.ApiUrl),
            Arg.Is(query.Deskey),
            Arg.Is(query.MD5key),
            Arg.Is(query.AgentId),
            Arg.Any<CancellationToken>())
            .Returns(credit);

        var service = new AgentBalanceGetService(
            fakeClient);

        var actual = await service.GetAsync(
            query: query);

        Assert.NotNull(actual);
        Assert.Equal(10000000.00m, actual.Credit);
    }
}
