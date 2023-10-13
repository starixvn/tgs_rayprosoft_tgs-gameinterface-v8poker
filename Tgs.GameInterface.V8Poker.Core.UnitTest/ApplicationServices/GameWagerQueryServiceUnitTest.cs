using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;
using Tgs.GameInterface.V8Poker.Core.Models;

namespace Tgs.GameInterface.V8Poker.Core.UnitTest.ApplicationServices;

public class GameWagerQueryServiceUnitTest
{
    [Fact]
    public async Task Player_Wins()
    {
        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://wc-record.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033",
            StartTime: DateTimeOffset.Parse("2023-07-26T11:50:00+08:00"),
            EndTime: DateTimeOffset.Parse("2023-07-26T12:00:00+08:00"));

        var fakeClient = Substitute.For<IGameRecordApiClient>();

        var infos = new GameRecordInfo[]
        {
            new GameRecordInfo(
                GameID: "50-1690343439-360242142-4",
                Account: "4ue0uj8cwn2v",
                ServerID: 9101,
                KindID: 910,
                TableID: 182020001,
                ChairID: 0,
                UserCount: 1,
                CellScore: 4.00m,
                AllBet: 4.00m,
                Profit: 4.00m,
                Revenue: 0,
                GameStartTime: DateTimeOffset.Parse("2023-07-26T11:50:39+08:00"),
                GameEndTime: DateTimeOffset.Parse("2023-07-26T11:51:02+08:00"),
                CardValue: "330b251a17001",
                ChannelID: 71033,
                LineCode: "71033_GPI",
                AdditionalData: new JsonObject
                {
                    ["GameID"] = "50-1690343439-360242142-4",
                    ["Accounts"] = "71033_4ue0uj8cwn2v",
                    ["ServerID"] = 9101,
                    ["KindID"] = 910,
                    ["TableID"] = 182020001,
                    ["ChairID"] = 0,
                    ["UserCount"] = 1,
                    ["CellScore"] =4.00m,
                    ["AllBet"] = 4.00m,
                    ["Profit"] = 4.00m,
                    ["Revenue"] = 0,
                    ["GameStartTime"] ="2023-07-26 11:50:39",
                    ["GameEndTime"] = "2023-07-26 11:51:02",
                    ["CardValue"] = "330b251a17001",
                    ["ChannelID"] =71033,
                    ["LineCode"] ="71033_GPI"
                }.ToJsonString()
                )
          };
        _ = fakeClient
            .GameRecordAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            deskey: Arg.Is(query.Deskey),
            md5key: Arg.Is(query.MD5key),
            agentId: Arg.Is(query.AgentId),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(infos.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.NotNull(actual);
        Assert.True(actual.Any());
        Assert.Equal(infos.First().GameID, actual.First().ExternalWagerId);
        Assert.Equal(infos.First().KindID.ToString(), actual.First().ExternalGameId);
        Assert.Equal(infos.First().Account, actual.First().PlayerCode);
        Assert.Equal(infos.First().AllBet, actual.First().Bet);
        Assert.Equal(infos.First().CellScore, actual.First().ValidBet);
        Assert.Equal(8.00m, actual.First().Settle);
        Assert.Equal(infos.First().Revenue, actual.First().Commission);
        Assert.Equal(infos.First().Profit, actual.First().WinLoss);
        Assert.Equal(infos.First().GameStartTime, actual.First().ExternalCreatedOn);
        Assert.Equal(infos.First().GameEndTime, actual.First().ExternalClosedOn);
        Assert.Equal(infos.First().AdditionalData, actual.First().AdditionalData);
    }

    [Fact]
    public async Task Player_Loses()
    {
        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://wc-record.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033",
            StartTime: DateTimeOffset.Parse("2023-07-17T16:55:00.000+08:00"),
            EndTime: DateTimeOffset.Parse("2023-07-17T17:00:00.000+08:00"));

        var fakeClient = Substitute.For<IGameRecordApiClient>();

        var infos = new GameRecordInfo[]
        {
            new GameRecordInfo(
                GameID: "686-1689584140-15524700-1",
                Account: "4ue0uj8cwn2v",
                ServerID: 9101,
                KindID: 910,
                TableID: 182020001,
                ChairID: 0,
                UserCount: 1,
                CellScore: 2.00m,
                AllBet: 2.00m,
                Profit: -2.00m,
                Revenue: 0,
                GameStartTime: DateTimeOffset.Parse("2023-07-17T16:55:40+08:00"),
                GameEndTime: DateTimeOffset.Parse("2023-07-17T16:56:02+08:00"),
                CardValue: "11150019163328",
                ChannelID: 71033,
                LineCode: "71033_GPI",
                AdditionalData: new JsonObject
                {
                    ["GameID"] = "686-1689584140-15524700-1",
                    ["Accounts"] = "71033_4ue0uj8cwn2v",
                    ["ServerID"] = 9101,
                    ["KindID"] = 910,
                    ["TableID"] = 182020001,
                    ["ChairID"] = 0,
                    ["UserCount"] = 1,
                    ["CellScore"] =2.00m,
                    ["AllBet"] = 2.00m,
                    ["Profit"] = -2.00m,
                    ["Revenue"] = 0,
                    ["GameStartTime"] ="2023-07-17 16:55:40",
                    ["GameEndTime"] = "2023-07-17 16:56:02",
                    ["CardValue"] = "11150019163328",
                    ["ChannelID"] =71033,
                    ["LineCode"] ="71033_GPI"
                }.ToJsonString()
                )
          };
        _ = fakeClient
            .GameRecordAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            deskey: Arg.Is(query.Deskey),
            md5key: Arg.Is(query.MD5key),
            agentId: Arg.Is(query.AgentId),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(infos.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.NotNull(actual);
        Assert.True(actual.Any());
        Assert.Equal(infos.First().GameID, actual.First().ExternalWagerId);
        Assert.Equal(infos.First().KindID.ToString(), actual.First().ExternalGameId);
        Assert.Equal(infos.First().Account, actual.First().PlayerCode);
        Assert.Equal(infos.First().AllBet, actual.First().Bet);
        Assert.Equal(infos.First().CellScore, actual.First().ValidBet);
        Assert.Equal(0, actual.First().Settle);
        Assert.Equal(infos.First().Revenue, actual.First().Commission);
        Assert.Equal(infos.First().Profit, actual.First().WinLoss);
        Assert.Equal(infos.First().GameStartTime, actual.First().ExternalCreatedOn);
        Assert.Equal(infos.First().GameEndTime, actual.First().ExternalClosedOn);
        Assert.Equal(infos.First().AdditionalData, actual.First().AdditionalData);
    }

    [Fact]
    public async Task Player_Versus_Banker_Versus_Player()
    {
        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://wc-record.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033",
            StartTime: DateTimeOffset.Parse("2023-07-26T11:50:00+08:00"),
            EndTime: DateTimeOffset.Parse("2023-07-26T12:00:00+08:00"));

        var fakeClient = Substitute.For<IGameRecordApiClient>();

        var infos = new GameRecordInfo[]
        {
            new GameRecordInfo(
                GameID: "50-1690343466-360242382-3",
                Account: "4ue0uj8cwn2v",
                ServerID: 9101,
                KindID: 910,
                TableID: 182020001,
                ChairID: 0,
                UserCount: 1,
                CellScore: 0.00m,
                AllBet: 8.00m,
                Profit: -0.20m,
                Revenue: 0,
                GameStartTime: DateTimeOffset.Parse("2023-07-26T11:51:06+08:00"),
                GameEndTime: DateTimeOffset.Parse("2023-07-26T11:51:29+08:00"),
                CardValue: "1b3c263b07002",
                ChannelID: 71033,
                LineCode: "71033_GPI",
                AdditionalData: new JsonObject
                {
                    ["GameID"] = "50-1690343466-360242382-3",
                    ["Accounts"] = "71033_4ue0uj8cwn2v",
                    ["ServerID"] = 9101,
                    ["KindID"] = 910,
                    ["TableID"] = 182020001,
                    ["ChairID"] = 0,
                    ["UserCount"] = 1,
                    ["CellScore"] =0.00m,
                    ["AllBet"] = 8.00m,
                    ["Profit"] = -0.20m,
                    ["Revenue"] = 0,
                    ["GameStartTime"] ="2023-07-26 11:51:06",
                    ["GameEndTime"] = "2023-07-26 11:51:29",
                    ["CardValue"] = "1b3c263b07002",
                    ["ChannelID"] =71033,
                    ["LineCode"] ="71033_GPI"
                }.ToJsonString()
                )
          };
        _ = fakeClient
            .GameRecordAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            deskey: Arg.Is(query.Deskey),
            md5key: Arg.Is(query.MD5key),
            agentId: Arg.Is(query.AgentId),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(infos.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.NotNull(actual);
        Assert.True(actual.Any());
        Assert.Equal(infos.First().GameID, actual.First().ExternalWagerId);
        Assert.Equal(infos.First().KindID.ToString(), actual.First().ExternalGameId);
        Assert.Equal(infos.First().Account, actual.First().PlayerCode);
        Assert.Equal(infos.First().AllBet, actual.First().Bet);
        Assert.Equal(infos.First().CellScore, actual.First().ValidBet);
        Assert.Equal(7.80m, actual.First().Settle);
        Assert.Equal(infos.First().Revenue, actual.First().Commission);
        Assert.Equal(infos.First().Profit, actual.First().WinLoss);
        Assert.Equal(infos.First().GameStartTime, actual.First().ExternalCreatedOn);
        Assert.Equal(infos.First().GameEndTime, actual.First().ExternalClosedOn);
        Assert.Equal(infos.First().AdditionalData, actual.First().AdditionalData);
    }

    [Fact]
    public async Task Player_Versus_Banker_Versus_Player_Versus_Tie()
    {
        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://wc-record.xpvcd046.com/"),
            Deskey: "C6995933499B163B",
            MD5key: "628DA8F1857A6529",
            AgentId: "71033",
            StartTime: DateTimeOffset.Parse("2023-07-18T11:59:00.000+08:00"),
            EndTime: DateTimeOffset.Parse("2023-07-18T12:09:00.000+08:00"));

        var fakeClient = Substitute.For<IGameRecordApiClient>();

        var infos = new GameRecordInfo[]
        {
            new GameRecordInfo(
                GameID: "686-1689653000-19588758-1",
                Account: "4ue0uj8cwn2v",
                ServerID: 9101,
                KindID: 910,
                TableID: 182020001,
                ChairID: 0,
                UserCount: 1,
                CellScore: 2.00m,
                AllBet: 6.00m,
                Profit: -2.00m,
                Revenue: 0,
                GameStartTime: DateTimeOffset.Parse("2023-07-18T12:03:20+08:00"),
                GameEndTime: DateTimeOffset.Parse("2023-07-18T12:03:44+08:00"),
                CardValue: "0605372c3d011",
                ChannelID: 71033,
                LineCode: "71033_GPI",
                AdditionalData: new JsonObject
                {
                    ["GameID"] = "686-1689653000-19588758-1",
                    ["Accounts"] = "71033_4ue0uj8cwn2v",
                    ["ServerID"] = 9101,
                    ["KindID"] = 910,
                    ["TableID"] = 182020001,
                    ["ChairID"] = 0,
                    ["UserCount"] = 1,
                    ["CellScore"] =2.00m,
                    ["AllBet"] = 6.00m,
                    ["Profit"] = -2.00m,
                    ["Revenue"] = 0,
                    ["GameStartTime"] ="2023-07-18 12:03:20",
                    ["GameEndTime"] = "2023-07-18 12:03:44",
                    ["CardValue"] = "0605372c3d011",
                    ["ChannelID"] =71033,
                    ["LineCode"] ="71033_GPI"
                }.ToJsonString()
                )
          };
        _ = fakeClient
            .GameRecordAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            deskey: Arg.Is(query.Deskey),
            md5key: Arg.Is(query.MD5key),
            agentId: Arg.Is(query.AgentId),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(infos.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.NotNull(actual);
        Assert.True(actual.Any());
        Assert.Equal(infos.First().GameID, actual.First().ExternalWagerId);
        Assert.Equal(infos.First().KindID.ToString(), actual.First().ExternalGameId);
        Assert.Equal(infos.First().Account, actual.First().PlayerCode);
        Assert.Equal(infos.First().AllBet, actual.First().Bet);
        Assert.Equal(infos.First().CellScore, actual.First().ValidBet);
        Assert.Equal(4m, actual.First().Settle);
        Assert.Equal(infos.First().Revenue, actual.First().Commission);
        Assert.Equal(infos.First().Profit, actual.First().WinLoss);
        Assert.Equal(infos.First().GameStartTime, actual.First().ExternalCreatedOn);
        Assert.Equal(infos.First().GameEndTime, actual.First().ExternalClosedOn);
        Assert.Equal(infos.First().AdditionalData, actual.First().AdditionalData);
    }
}
