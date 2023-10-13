namespace Tgs.GameInterface.V8Poker.ViewModels;

public record GameRecordResultViewModel(
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
