namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

public record PlayerBalanceQuery(
	Uri ApiUrl,
	string Deskey,
	string MD5key,
	string AgentId,
	string PlayerCode);
