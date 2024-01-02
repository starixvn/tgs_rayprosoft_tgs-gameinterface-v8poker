using System.Runtime.CompilerServices;
using Tgs.GameInterface.V8Poker.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

internal class GameWagerQueryService(
    IGameRecordApiClient client) : IQueryService<GameWagerQuery, GameWagerResult>
{
    public async IAsyncEnumerable<GameWagerResult> QueryAsync(
        GameWagerQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var gameRecords = await client
            .GameRecordAsync(
            apiUrl: query.ApiUrl,
            deskey: query.Deskey,
            md5key: query.MD5key,
            agentId: query.AgentId,
            startTime: query.StartTime,
            endTime: query.EndTime,
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameRecordInfo>();

        if (!gameRecords.Any())
        {
            yield break;
        }

        foreach (var gameRecord in gameRecords)
        {
            yield return new GameWagerResult(
                ExternalWagerId: gameRecord.GameID,
                ExternalGameId: gameRecord.KindID.ToString(),
                PlayerCode: gameRecord.Account,
                ExternalCreatedOn: gameRecord.GameStartTime,
                ExternalClosedOn: gameRecord.GameEndTime,
                Bet: gameRecord.AllBet,
                ValidBet: gameRecord.CellScore,
                Settle: gameRecord.AllBet + gameRecord.Profit,
                Commission: gameRecord.Revenue,
                WinLoss: gameRecord.Profit,
                AdditionalData: gameRecord.AdditionalData);
        }
    }
}
