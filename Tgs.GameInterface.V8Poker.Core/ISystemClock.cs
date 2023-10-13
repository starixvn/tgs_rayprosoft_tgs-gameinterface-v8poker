namespace Tgs.GameInterface.V8Poker.Core;

public interface ISystemClock
{
    DateTimeOffset CurrentTime { get; }
}
