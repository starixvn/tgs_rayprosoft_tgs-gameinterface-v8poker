namespace Tgs.GameInterface.V8Poker.Core.Models;

public record GameRecordInfo(
	string GameID,
	string Account,
	int ServerID,
	int KindID,
	long TableID,
	int? ChairID,
	int UserCount,
	decimal CellScore,
	decimal AllBet,
	decimal Profit,
	decimal Revenue,
	DateTimeOffset GameStartTime,
	DateTimeOffset GameEndTime,
	string CardValue,
	int ChannelID,
	string LineCode,
	string AdditionalData);
