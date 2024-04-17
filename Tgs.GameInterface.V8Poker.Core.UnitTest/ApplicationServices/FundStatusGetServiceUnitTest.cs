using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
	[Fact]
	public async Task Normal_ProcessAsync()
	{
		var query = new FundStatusQuery(
			ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
			Deskey: "C6995933499B163B",
			MD5key: "628DA8F1857A6529",
			AgentId: "71033",
			OrderId: "71033202307260300148304ue0uj8cwn2v");

		var fakeClient = Substitute.For<IGameApiClient>();

		var service = new FundStatusGetService(
			fakeClient);

		_ = await service.GetAsync(
			query: query);

		_ = fakeClient
			.Received(1)
			.GetOrderAsync(
			Arg.Is(query.ApiUrl),
			Arg.Is(query.Deskey),
			Arg.Is(query.MD5key),
			Arg.Is(query.AgentId),
			Arg.Is(query.OrderId),
			Arg.Any<CancellationToken>());
	}
}
