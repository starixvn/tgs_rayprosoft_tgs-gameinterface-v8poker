using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;
using Tgs.GameInterface.V8Poker.Core.Models;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class FundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_ProcessAsync()
	{
		var query = new FundOutQuery(
			ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
			Deskey: "C6995933499B163B",
			MD5key: "628DA8F1857A6529",
			AgentId: "71033",
			PlayerCode: "4ue0uj8cwn2v",
			OrderId: "7103320202310120336009404ue0uj8cwn2v",
			Amount: 1.0m);

		var fakeClient = Substitute.For<IGameApiClient>();

		_ = fakeClient
			.DepositAsync(
			Arg.Is(query.ApiUrl),
			Arg.Is(query.Deskey),
			Arg.Is(query.MD5key),
			Arg.Is(query.AgentId),
			Arg.Is(query.PlayerCode),
			Arg.Is(query.OrderId),
			Arg.Is(query.Amount),
			Arg.Any<CancellationToken>())
			.Returns(new ValueTask<DepositResp>(
				new DepositResp(Resp: "{\"m\":\"/channelHandle\",\"s\":102,\"d\":{\"code\":0,\"money\":\"1.00\"}}")));

		var service = new FundOutGetService(
			fakeClient);

		var actual = await service.GetAsync(query: query);

		Assert.Equal("{\"m\":\"/channelHandle\",\"s\":102,\"d\":{\"code\":0,\"money\":\"1.00\"}}", actual.Resp);
	}
}
