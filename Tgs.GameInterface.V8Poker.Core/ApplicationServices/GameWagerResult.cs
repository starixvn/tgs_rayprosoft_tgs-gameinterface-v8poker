namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

public record GameWagerResult(
	string ExternalWagerId,
	string ExternalGameId,
	string PlayerCode,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal WinLoss,
	string AdditionalData);
