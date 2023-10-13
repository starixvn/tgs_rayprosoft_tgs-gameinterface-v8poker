using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest;

public class BalanceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        Uri apiUrl = new Uri("https://wc-api.xpvcd046.com/");
        var deskey = "C6995933499B163B";
        var md5key = "628DA8F1857A6529";
        var agentId = "71033";
        var account = "4ue0uj8cwn2v";

        var fakeCryptography = Substitute.For<IHashCryptography>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = DateTimeOffset.FromUnixTimeMilliseconds(1690340035435);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var data = "s=7&account=4ue0uj8cwn2v";
        var param = "TVvTX3VbPPF8uoTUB9GG382q5wLB1kCJjq8gYecYEyc=";
        _ = fakeCryptography.GetAesEncryptAsync(
            data: Arg.Is(data),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(param);

        var dataMD5 = $"{agentId}{currentTime.ToUnixTimeMilliseconds()}{md5key}";
        var key = "2a9a46e4cb2ef009ce1ca3732227ec4f";
        _ = fakeCryptography.GetHash(
            data: Arg.Is(dataMD5))
            .Returns(key);

        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/channelHandle"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["agent"] = agentId;
        query["timestamp"] = currentTime.ToUnixTimeMilliseconds().ToString();
        query["param"] = param;
        query["key"] = key;
        uriBuilder.Query = query.ToString();

        var response = new JsonObject
        {
            ["m"] = "/channelHandle",
            ["s"] = 107,
            ["d"] = new JsonObject
            {
                ["code"] = 0,
                ["account"] = "71033_4ue0uj8cwn2v",
                ["totalMoney"] = 1,
                ["freeMoney"] = 1,
                ["status"] = 0,
                ["gameStatus"] = 0
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Get, uriBuilder.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var client = new GameApiClient(
            fakeLog,
            fakeCryptography,
            fakeSystemClock,
            fakeHttpClient);

        var actual = await client.BalanceAsync(
            apiUrl: apiUrl,
            deskey: deskey,
            md5key: md5key,
            agentId: agentId,
            account: account);

        Assert.NotNull(actual);
        Assert.Equal(1m, actual.FreeMoney);
    }
}
