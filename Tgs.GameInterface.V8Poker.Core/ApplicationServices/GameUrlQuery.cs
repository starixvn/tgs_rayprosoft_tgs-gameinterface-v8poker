namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

public record GameUrlQuery(
	Uri ApiUrl,
	string Deskey,
	string MD5key,
	string AgentId,
	string BrandCode,
	string PlayerCode,
	string IpAddress,
	string ExternalGameId,
	string Lang);
