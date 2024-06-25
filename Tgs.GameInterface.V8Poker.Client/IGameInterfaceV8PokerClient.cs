using Tgs.GameInterface.V8Poker.Protos;

namespace Tgs.GameInterface.V8Poker.Client;

public interface IGameInterfaceV8PokerClient
{
	ValueTask<AgentBalanceReply> AgentBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, CancellationToken cancellationToken = default);

	ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundInAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundOutAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<bool> FundStatusAsync(Uri apiUrl, string deskey, string md5key, string agentId, string orderId, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerInfo> GameWagerAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);

	ValueTask<string> GameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, string brandCode, string playerCode, string ipAddress, string externalGameId, string lang, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerLogoutAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, CancellationToken cancellationToken = default);
}
