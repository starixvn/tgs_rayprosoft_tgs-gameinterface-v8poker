namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

public record FundOutQuery(
    Uri ApiUrl,
    string Deskey,
    string MD5key,
    string AgentId,
    string PlayerCode,
    string OrderId,
    decimal Amount);
