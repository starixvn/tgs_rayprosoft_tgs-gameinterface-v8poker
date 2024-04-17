using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;
using Tgs.GameInterface.V8Poker.Core.Models;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_ProcessAsync()
	{
		var query = new PlayerBalanceQuery(
			ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
			Deskey: "C6995933499B163B",
			MD5key: "628DA8F1857A6529",
			AgentId: "71033",
			PlayerCode: "4ue0uj8cwn2v");

		var fakeClient = Substitute.For<IGameApiClient>();

		var info = new BalanceInfo(
			TotalMoney: 100,
			FreeMoney: 98);
		_ = fakeClient
			.BalanceAsync(
			Arg.Is(query.ApiUrl),
			Arg.Is(query.Deskey),
			Arg.Is(query.MD5key),
			Arg.Is(query.AgentId),
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(info);

		var service = new PlayerBalanceGetService(
			fakeClient);

		var actual = await service.GetAsync(
			query: query);

		Assert.NotNull(actual);
		Assert.Equal(100, actual.Balance);
		Assert.Equal(98, actual.FreeBalance);
		Assert.Equal(2, actual.OutstandingBalance);
	}
}
