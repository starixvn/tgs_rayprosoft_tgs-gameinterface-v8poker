using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;
using Tgs.GameInterface.V8Poker.Core.Models;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class PlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_ProcessAsync()
	{
		var query = new PlayerOnlineQuery(
			ApiUrl: new Uri("https://wc-api.xpvcd046.com/"),
			Deskey: "C6995933499B163B",
			MD5key: "628DA8F1857A6529",
			AgentId: "71033",
			PlayerCode: "4ue0uj8cwn2v");

		var fakeClient = Substitute.For<IGameApiClient>();

		_ = fakeClient.PlayerOnlineAsync(
			Arg.Is(query.ApiUrl),
			Arg.Is(query.Deskey),
			Arg.Is(query.MD5key),
			Arg.Is(query.AgentId),
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(true);

		var service = new PlayerOnlineGetService(
			fakeClient);

		var actual = await service.GetAsync(
			query: query);

		Assert.True(actual);
	}
}
