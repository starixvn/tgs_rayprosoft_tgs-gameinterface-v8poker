using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest;

public record WithdrawalUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			GameRecordRetryCount = 1,
			GameRecordRetryWaitSec = 1
		});

		Uri apiUrl = new Uri("https://wc-api.xpvcd046.com/");
		var deskey = "C6995933499B163B";
		var md5key = "628DA8F1857A6529";
		var agentId = "71033";
		var account = "4ue0uj8cwn2v";
		var orderId = "71033202310121000161234ue0uj8cwn2v";
		var money = 1.0m;

		var fakeCryptography = Substitute.For<IHashCryptography>();
		var fakeSystemClock = Substitute.For<ISystemClock>();

		var currentTime = DateTimeOffset.Parse("2023-07-26T03:44:19.406+00:00");
		_ = fakeSystemClock.CurrentTime.Returns(currentTime);

		var data = "s=3&account=4ue0uj8cwn2v&money=1.0&orderid=71033202310121000161234ue0uj8cwn2v";
		var param = "NnlQBQWvhxyimD+GgoNvmseLG54H1Gp7fhoksdXiGpleMvtAAFzHCFIFakM5X0PuTPy2S/JC1TdJCuTs1/i8nx//NefC1YiGKxBINX1K/v4=";
		_ = fakeCryptography.GetAesEncryptAsync(
			data: Arg.Is(data),
			secretKey: Arg.Is(deskey),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(param);

		var dataMD5 = $"{agentId}{currentTime.ToUnixTimeMilliseconds()}{md5key}";
		var key = "a21bbf4a5e3d4137ae91f5623fc735a6";
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
			["s"] = 103,
			["d"] = new JsonObject
			{
				["code"] = 0,
				["money"] = "1.00"
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
			policySettingsOptions,
			fakeCryptography,
			fakeSystemClock,
			fakeHttpClient);

		var actual = await client.WithdrawAsync(
			apiUrl: apiUrl,
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			account: account,
			orderId: orderId,
			money: money);

		Assert.Equal(response.ToJsonString(), actual.Resp);
	}
}
