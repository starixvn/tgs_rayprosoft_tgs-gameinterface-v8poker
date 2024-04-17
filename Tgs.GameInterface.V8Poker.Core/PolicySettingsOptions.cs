namespace Tgs.GameInterface.V8Poker.Core;

public record PolicySettingsOptions
{
	/// <summary>
	/// The deposit retry count
	/// </summary>
	public int DepositRetryCount { get; init; }
	/// <summary>
	/// The deposit retry wait sec
	/// </summary>
	public int DepositRetryWaitSec { get; set; }
	/// <summary>
	/// The game record retry count
	/// </summary>
	public int GameRecordRetryCount { get; set; }
	/// <summary>
	/// The game record retry wait sec
	/// </summary>
	public int GameRecordRetryWaitSec { get; set; }
}
