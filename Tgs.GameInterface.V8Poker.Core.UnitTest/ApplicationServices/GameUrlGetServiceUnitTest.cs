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
            Lang: "en-us");

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
            .Returns("https://wc-h5.orz16.com?account=71033_4ue0uj8cwn2v&token=eyJkYXRhIjoiNjAwNzNfNHVoM3BkbDBsY2lpIiwiY3JlYXRlZCI6MTY5NzA3Nzg2MywiZXhwIjozMDAsImFjY0xldmVsIjoiTFY1In0=.NYUG96iiOAfKinF/cZ7H03nIKt1+MI/TGfquo6BilPM=&lang=zh-cn&defaultSkin=[4]&route=wss://wc-wss.orz16.com&loudou=https://wc-loud.orz16.com/statisticsHandle&moneyType=1");

        var service = new GameUrlGetService(
            fakeClient);

        var actual = await service.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Contains("lang=en-us", actual);
        Assert.DoesNotContain("lang=zh-cn", actual);
    }
}
