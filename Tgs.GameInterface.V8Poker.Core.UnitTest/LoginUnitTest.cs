using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest;

public class LoginUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var apiUrl = new Uri("https://wc-api.xpvcd046.com/");
        var deskey = "C6995933499B163B";
        var md5key = "628DA8F1857A6529";
        var agentId = "71033";
        var account = "4ue0uj8cwn2v";
        var ipAddress = "127.0.0.1";
        var lineCode = "GPI_VND";
        var kindId = "0";

        var fakeCryptography = Substitute.For<IHashCryptography>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = DateTimeOffset.FromUnixTimeMilliseconds(1697077837833);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var data = "s=0&account=4ue0uj8cwn2v&money=0&orderid=71033202310120230378334ue0uj8cwn2v&ip=127.0.0.1&lineCode=GPI_VND&KindID=0";
        var param = "oCKRHPQ9LbMrbqxJuK4bxyOm7cc2MfVKCeUI8RbrEjv7d96Xzf0qRYpwY/vtEHrySj0RSCU7xkI7Q1zC12AFXwq231Henc7KgnXSZVzXdjVEr2EU5kuvRoQGLJ95REr0+2Fa41CE66wfFZQsriBNGg==";
        _ = fakeCryptography.GetAesEncryptAsync(
            data: Arg.Is(data),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(param);

        var dataMD5 = $"{agentId}{currentTime.ToUnixTimeMilliseconds()}{md5key}";
        var key = "2f72cd7b2bc87c60f2b9dd6e45268097";
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
            ["s"] = 100,
            ["d"] = new JsonObject
            {
                ["code"] = 0,
                ["url"] = "https://wc-h5.orz16.com/",
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

        var actual = await client
            .LoginAsync(
            apiUrl: apiUrl,
            deskey: deskey,
            md5key: md5key,
            agentId: agentId,
            account: account,
            ipAddress: ipAddress,
            lineCode: lineCode,
            kindId: kindId);

        Assert.NotNull(actual);
        Assert.Equal("https://wc-h5.orz16.com/", actual);
    }
}
