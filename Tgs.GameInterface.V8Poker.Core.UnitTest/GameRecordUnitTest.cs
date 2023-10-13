using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest;

public class GameRecordUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        Uri apiUrl = new Uri("https://wc-record.xpvcd046.com/");
        var deskey = "C6995933499B163B";
        var md5key = "628DA8F1857A6529";
        var agentId = "71033";
        var startTime = DateTimeOffset.Parse("2023-07-26T11:50:00+08:00");
        var endTime = DateTimeOffset.Parse("2023-07-26T12:00:00+08:00");

        var fakeCryptography = Substitute.For<IHashCryptography>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = DateTimeOffset.FromUnixTimeMilliseconds(1690343754287);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var data = "s=6&startTime=1690343400000&endTime=1690344000000";
        var param = "H5kLMUYwNlqbCtfaD/H4qyLglUI3Y41FMtJcy5EyW76A1DDk3BXpPlgKjnDo/XqdVHZOgPcUHBwewVlMD0cssg==";
        _ = fakeCryptography.GetAesEncryptAsync(
            data: Arg.Is(data),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(param);

        var dataMD5 = $"{agentId}{currentTime.ToUnixTimeMilliseconds()}{md5key}";
        var key = "4d6bcf79f2b72fe88d484e65e8606e94";
        _ = fakeCryptography.GetHash(
            data: Arg.Is(dataMD5))
            .Returns(key);

        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/getRecordHandle"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["agent"] = agentId;
        query["timestamp"] = currentTime.ToUnixTimeMilliseconds().ToString();
        query["param"] = param;
        query["key"] = key;
        uriBuilder.Query = query.ToString();

        var response = new JsonObject
        {
            ["m"] = "/getRecordHandle",
            ["s"] = 106,
            ["d"] = new JsonObject
            {
                ["code"] = 0,
                ["start"] = 1690343400000,
                ["end"] = 1690344000000,
                ["count"] = 2,
                ["list"] = new JsonObject
                {
                    ["GameID"] = new JsonArray {
                        "50-1690343439-360242142-4",
                        "50-1690343466-360242382-3"
                    },
                    ["Accounts"] = new JsonArray {
                        "71033_4ue0uj8cwn2v",
                        "71033_4ue0uj8cwn2v"
                    },
                    ["ServerID"] = new JsonArray {
                        9101,
                        9101
                    },
                    ["KindID"] = new JsonArray {
                        910,
                        910
                    },
                    ["TableID"] = new JsonArray {
                        182020001,
                        182020001
                    },
                    ["ChairID"] = new JsonArray {
                        0,
                        0
                    },
                    ["UserCount"] = new JsonArray {
                        1,
                        1
                    },
                    ["CellScore"] = new JsonArray {
                        "4.00",
                        "0.00"
                    },
                    ["AllBet"] = new JsonArray {
                        "4.00",
                        "8.00"
                    },
                    ["Profit"] = new JsonArray {
                        "4.00",
                        "-0.20"
                    },
                    ["Revenue"] = new JsonArray {
                        "0.00",
                        "0.00"
                    },
                    ["GameStartTime"] = new JsonArray {
                        "2023-07-26 11:50:39",
                        "2023-07-26 11:51:06"
                    },
                    ["GameEndTime"] = new JsonArray {
                        "2023-07-26 11:51:02",
                        "2023-07-26 11:51:29"
                    },
                    ["CardValue"] = new JsonArray {
                        "330b251a17001",
                        "1b3c263b07002"
                    },
                    ["ChannelID"] = new JsonArray {
                        71033,
                        71033
                    },
                    ["LineCode"] = new JsonArray {
                        "71033_GPI",
                        "71033_GPI"
                    }
                }
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Get, uriBuilder.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameRecordApiClient>.Instance;

        var client = new GameRecordApiClient(
            fakeLog,
            fakeCryptography,
            fakeSystemClock,
            fakeHttpClient);

        var actual = await client
            .GameRecordAsync(
            apiUrl: apiUrl,
            deskey: deskey,
            md5key: md5key,
            agentId: agentId,
            startTime: startTime,
            endTime: endTime,
            retrySeconds: 1)
            .ToArrayAsync();

        Assert.NotNull(actual);
        Assert.True(actual.Any());
        Assert.Equal("50-1690343439-360242142-4", actual.First().GameID);
        Assert.Equal("4ue0uj8cwn2v", actual.First().Account);
        Assert.Equal(9101, actual.First().ServerID);
        Assert.Equal(910, actual.First().KindID);
        Assert.Equal(182020001, actual.First().TableID);
        Assert.Equal(0, actual.First().ChairID);
        Assert.Equal(1, actual.First().UserCount);
        Assert.Equal(4.00m, actual.First().CellScore);
        Assert.Equal(4.00m, actual.First().AllBet);
        Assert.Equal(4.00m, actual.First().Profit);
        Assert.Equal(0.00m, actual.First().Revenue);
        Assert.Equal(DateTimeOffset.Parse("2023-07-26T11:50:39+08:00"), actual.First().GameStartTime);
        Assert.Equal(DateTimeOffset.Parse("2023-07-26T11:51:02+08:00"), actual.First().GameEndTime);
        Assert.Equal("330b251a17001", actual.First().CardValue);
        Assert.Equal(71033, actual.First().ChannelID);
        Assert.Equal("71033_GPI", actual.First().LineCode);
    }

    [Fact]
    public async void Data_Does_Not_Exist_Code_16()
    {
        Uri apiUrl = new Uri("https://wc-record.xpvcd046.com/");
        var deskey = "C6995933499B163B";
        var md5key = "628DA8F1857A6529";
        var agentId = "71033";
        var startTime = DateTimeOffset.Parse("2023-07-26T13:00:00+08:00");
        var endTime = DateTimeOffset.Parse("2023-07-26T13:05:00+08:00");

        var fakeCryptography = Substitute.For<IHashCryptography>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = DateTimeOffset.FromUnixTimeMilliseconds(1690349319069);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var data = "s=6&startTime=1690347600000&endTime=1690347900000";
        var param = "H5kLMUYwNlqbCtfaD/H4q3isFDPb/RE4jdnQxnYreE0C957QWEzl2p/4YAddsTJIVHZOgPcUHBwewVlMD0cssg==";
        _ = fakeCryptography.GetAesEncryptAsync(
            data: Arg.Is(data),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(param);

        var dataMD5 = $"{agentId}{currentTime.ToUnixTimeMilliseconds()}{md5key}";
        var key = "a57d07b85d6ffe0c9d1f4223830a1c6f";
        _ = fakeCryptography.GetHash(
            data: Arg.Is(dataMD5))
            .Returns(key);

        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/getRecordHandle"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["agent"] = agentId;
        query["timestamp"] = currentTime.ToUnixTimeMilliseconds().ToString();
        query["param"] = param;
        query["key"] = key;
        uriBuilder.Query = query.ToString();

        var response = new JsonObject
        {
            ["m"] = "/getRecordHandle",
            ["s"] = 106,
            ["d"] = new JsonObject
            {
                ["code"] = 16,
                ["start"] = 1690347600000,
                ["end"] = 1690347900000,
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Get, uriBuilder.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameRecordApiClient>.Instance;

        var client = new GameRecordApiClient(
            fakeLog,
            fakeCryptography,
            fakeSystemClock,
            fakeHttpClient);

        var actual = await client
            .GameRecordAsync(
            apiUrl: apiUrl,
            deskey: deskey,
            md5key: md5key,
            agentId: agentId,
            startTime: startTime,
            endTime: endTime,
            retrySeconds: 1)
            .ToArrayAsync();

        Assert.NotNull(actual);
        Assert.False(actual.Any());
    }

    [Fact]
    public async void Pulling_Orders_Too_Frequently_Coode_43()
    {
        Uri apiUrl = new Uri("https://wc-record.xpvcd046.com/");
        var deskey = "C6995933499B163B";
        var md5key = "628DA8F1857A6529";
        var agentId = "71033";
        var startTime = DateTimeOffset.Parse("2023-07-26T13:10:00+08:00");
        var endTime = DateTimeOffset.Parse("2023-07-26T13:20:00+08:00");

        var fakeCryptography = Substitute.For<IHashCryptography>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = DateTimeOffset.FromUnixTimeMilliseconds(1690348568235);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var data = "s=6&startTime=1690348200000&endTime=1690348800000";
        var param = "H5kLMUYwNlqbCtfaD/H4q5uYqcpmxo7iGbVFQVrZfi0LKUOD2P68jf+FGh6DbZdMVHZOgPcUHBwewVlMD0cssg==";
        _ = fakeCryptography.GetAesEncryptAsync(
            data: Arg.Is(data),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(param);

        var dataMD5 = $"{agentId}{currentTime.ToUnixTimeMilliseconds()}{md5key}";
        var key = "42fea6d9cb7fc7260e09e97ef4b9f33b";
        _ = fakeCryptography.GetHash(
            data: Arg.Is(dataMD5))
            .Returns(key);

        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/getRecordHandle"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["agent"] = agentId;
        query["timestamp"] = currentTime.ToUnixTimeMilliseconds().ToString();
        query["param"] = param;
        query["key"] = key;
        uriBuilder.Query = query.ToString();

        var response = new JsonObject
        {
            ["m"] = "/getRecordHandle",
            ["s"] = 106,
            ["d"] = new JsonObject
            {
                ["code"] = 43
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Get, uriBuilder.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameRecordApiClient>.Instance;

        var client = new GameRecordApiClient(
            fakeLog,
            fakeCryptography,
            fakeSystemClock,
            fakeHttpClient);

        _ = await Assert.ThrowsAsync<HttpRequestException>(
            () => client
            .GameRecordAsync(
                apiUrl: apiUrl,
                deskey: deskey,
                md5key: md5key,
                agentId: agentId,
                startTime: startTime,
                endTime: endTime,
                retrySeconds: 1)
            .ToArrayAsync()
            .AsTask());
    }

    [Fact]
    public async void Fishing()
    {
        Uri apiUrl = new Uri("https://wc-record.xpvcd046.com/");
        var deskey = "C6995933499B163B";
        var md5key = "628DA8F1857A6529";
        var agentId = "71033";
        var startTime = DateTimeOffset.Parse("2023-07-26T13:10:00+08:00");
        var endTime = DateTimeOffset.Parse("2023-07-26T13:20:00+08:00");

        var fakeCryptography = Substitute.For<IHashCryptography>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = DateTimeOffset.FromUnixTimeMilliseconds(1690348568235);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var data = "s=6&startTime=1690348200000&endTime=1690348800000";
        var param = "H5kLMUYwNlqbCtfaD/H4q5uYqcpmxo7iGbVFQVrZfi0LKUOD2P68jf+FGh6DbZdMVHZOgPcUHBwewVlMD0cssg==";
        _ = fakeCryptography.GetAesEncryptAsync(
            data: Arg.Is(data),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(param);

        var dataMD5 = $"{agentId}{currentTime.ToUnixTimeMilliseconds()}{md5key}";
        var key = "42fea6d9cb7fc7260e09e97ef4b9f33b";
        _ = fakeCryptography.GetHash(
            data: Arg.Is(dataMD5))
            .Returns(key);

        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/getRecordHandle"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["agent"] = agentId;
        query["timestamp"] = currentTime.ToUnixTimeMilliseconds().ToString();
        query["param"] = param;
        query["key"] = key;
        uriBuilder.Query = query.ToString();

        var response = new JsonObject
        {
            ["m"] = "/getRecordHandle",
            ["s"] = 106,
            ["d"] = new JsonObject
            {
                ["code"] = 0,
                ["start"] = 1690348200000,
                ["end"] = 1690348800000,
                ["count"] = 1,
                ["list"] = new JsonObject
                {
                    ["GameID"] = new JsonArray {
                        "50-1690348239-360284844-2"
                    },
                    ["Accounts"] = new JsonArray {
                        "71033_4ue0uj8cwn2v"
                    },
                    ["ServerID"] = new JsonArray {
                        5401
                    },
                    ["KindID"] = new JsonArray {
                        540
                    },
                    ["TableID"] = new JsonArray {
                        108020001
                    },
                    ["ChairID"] = new JsonArray {
                        0
                    },
                    ["UserCount"] = new JsonArray {
                        1
                    },
                    ["CellScore"] = new JsonArray {
                        "259.80"
                    },
                    ["AllBet"] = new JsonArray {
                        "259.80"
                    },
                    ["Profit"] = new JsonArray {
                        "-103.10"
                    },
                    ["Revenue"] = new JsonArray {
                        "0.00"
                    },
                    ["GameStartTime"] = new JsonArray {
                        "2023-07-26 13:10:39"
                    },
                    ["GameEndTime"] = new JsonArray {
                        "2023-07-26 13:12:27"
                    },
                    ["CardValue"] = new JsonArray {
                        "0"
                    },
                    ["ChannelID"] = new JsonArray {
                        71033
                    },
                    ["LineCode"] = new JsonArray {
                        "71033_GPI"
                    }
                }
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Get, uriBuilder.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameRecordApiClient>.Instance;

        var client = new GameRecordApiClient(
            fakeLog,
            fakeCryptography,
            fakeSystemClock,
            fakeHttpClient);

        var actual = await client
            .GameRecordAsync(
            apiUrl: apiUrl,
            deskey: deskey,
            md5key: md5key,
            agentId: agentId,
            startTime: startTime,
            endTime: endTime,
            retrySeconds: 1)
            .ToArrayAsync();

        Assert.NotNull(actual);
        Assert.True(actual.Any());
        Assert.Equal("50-1690348239-360284844-2", actual.First().GameID);
        Assert.Equal("4ue0uj8cwn2v", actual.First().Account);
        Assert.Equal(5401, actual.First().ServerID);
        Assert.Equal(540, actual.First().KindID);
        Assert.Equal(108020001, actual.First().TableID);
        Assert.Equal(0, actual.First().ChairID);
        Assert.Equal(1, actual.First().UserCount);
        Assert.Equal(259.80m, actual.First().CellScore);
        Assert.Equal(259.80m, actual.First().AllBet);
        Assert.Equal(-103.10m, actual.First().Profit);
        Assert.Equal(0.00m, actual.First().Revenue);
        Assert.Equal(DateTimeOffset.Parse("2023-07-26T13:10:39+08:00"), actual.First().GameStartTime);
        Assert.Equal(DateTimeOffset.Parse("2023-07-26T13:12:27+08:00"), actual.First().GameEndTime);
        Assert.Equal("0", actual.First().CardValue);
        Assert.Equal(71033, actual.First().ChannelID);
        Assert.Equal("71033_GPI", actual.First().LineCode);
    }
}
