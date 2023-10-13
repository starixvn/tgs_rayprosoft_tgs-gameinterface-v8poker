namespace Tgs.GameInterface.V8Poker.ViewModels;

public record PlayerBalanceResultViewModel(
    decimal Balance,
    decimal FreeBalance,
    decimal OutstandingBalance);
