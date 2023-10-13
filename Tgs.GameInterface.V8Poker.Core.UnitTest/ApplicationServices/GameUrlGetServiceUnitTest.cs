using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class GameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_ProcessAsync()
    {
        var query = new GameUrlQuery(
            ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033",
            BrandCode: "GPI",
            PlayerCode: "4ue0uj8cwn2v",
            IpAddress: "127.0.0.1",
            ExternalGameId: "0",
            Lang: "en_us");

        var fakeClient = Substitute.For<IGameApiClient>();

        _ = fakeClient
            .LoginAsync(
            Arg.Is(query.ApiUrl),
            Arg.Is(query.Deskey),
            Arg.Is(query.MD5key),
            Arg.Is(query.AgentId),
            Arg.Is(query.PlayerCode),
            Arg.Is(query.IpAddress),
            Arg.Is(query.BrandCode),
            Arg.Is(query.ExternalGameId),
            Arg.Any<CancellationToken>())
            .Returns("https://wc-h5.xpvcd046.com/index.html?account=71033_4ue0uj8cwn2v&token=eyJkYXRhIjoiNzEwMzNfNHVlMHVqOGN3bjJ2IiwiY3JlYXRlZCI6MTY5NzE3NzUxMCwiZXhwIjozMDB9.kiC/YUrtK9cD8pPTL73q8G2ym6JK12ZvE8udIADrn8I=&route=wc-wss.xpvcd046.com&ld=https://wc-loud.xpvcd046.com/statisticsHandle&gameId=0&lang=vi_vn&extraParam=5d000001003500000000000000003d888a8693b62398eee1795f1a25e266c6716946a8d438f2e982d66b8c4186add90fbb7cc166dc4e88bf5d40996c4722b6fdaa4789ed9dc3abeffff4892c00");

        var service = new GameUrlGetService(
            fakeClient);

        var actual = await service.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Contains("lang=en_us", actual);
        Assert.DoesNotContain("lang=vi_vn", actual);
    }
}
