using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameInterface.V8Poker.Core.Exceptions;
using Tgs.GameInterface.V8Poker.Core.Extensions;
using Tgs.GameInterface.V8Poker.Core.Models;

namespace Tgs.GameInterface.V8Poker.Core;

internal class GameRecordApiClient : IGameRecordApiClient
{
    private readonly ILogger<GameRecordApiClient> _logger;
    private readonly PolicySettingsOptions _policySettingsOptions;
    private readonly IHashCryptography _cryptography;
    private readonly ISystemClock _systemClock;
    private readonly HttpClient _client;

    public GameRecordApiClient(
        ILogger<GameRecordApiClient> logger,
        IOptions<PolicySettingsOptions> options,
        IHashCryptography cryptography,
        ISystemClock systemClock,
        HttpClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _policySettingsOptions = options.Value;
        _cryptography = cryptography ?? throw new ArgumentNullException(nameof(cryptography));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<GameRecordInfo> GameRecordAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset startTime, DateTimeOffset endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var retryPolicy = Policy
            .HandleResult<JsonNode>(jsonNode => jsonNode["d"]!["code"]!.GetValue<int>() == 43)
            .WaitAndRetryAsync(
            retryCount: _policySettingsOptions.GameRecordRetryCount,
            retryAttempt => TimeSpan.FromSeconds(_policySettingsOptions.GameRecordRetryWaitSec),
            onRetry: (response, retryTime, context) =>
            {
                _logger.LogWarning("LogOn:{logOn} | Message:拉單過於頻繁(兩次拉單時間間隔必須大於 10 秒 | Json:{jsonNode}", DateTime.Now.ToString("o"), response.Result.ToJsonString());
            });

        var data = $"s=6&startTime={startTime.ToUnixTimeMilliseconds()}&endTime={endTime.ToUnixTimeMilliseconds()}";
        var jsonNode = await retryPolicy
            .ExecuteAsync(
            async () => await GetAsync(
                apiUrl: apiUrl,
                logPrefix: "QueryGameOrder",
                deskey: deskey,
                md5key: md5key,
                agentId: agentId,
                data: data,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false))
            .ConfigureAwait(false);

        var dataResponse = jsonNode["d"]!;

        if (dataResponse["code"]!.GetValue<int>() != 0)
        {
            if (dataResponse["code"]!.GetValue<int>() is 16 or 43)
            {
                yield break;
            }
            throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>());
        }

        var list = dataResponse["list"];

        if (list is null)
        {
            yield break;
        }

        var gameIds = list["GameID"]!.AsArray().Select(x => x!.ToString());
        var accounts = list["Accounts"]!.AsArray().Select(x => x!.ToString());
        var serverIds = list["ServerID"]!.AsArray().Select(x => Convert.ToInt32(x!.ToString()));
        var kindIds = list["KindID"]!.AsArray().Select(x => Convert.ToInt32(x!.ToString()));
        var tableIds = list["TableID"]!.AsArray().Select(x => Convert.ToInt64(x!.ToString()));
        var chairIds = list["ChairID"]!.AsArray().Select(x => (Convert.IsDBNull(x)) ? (int?)null : Convert.ToInt32(x!.ToString()));
        var userCount = list["UserCount"]!.AsArray().Select(x => Convert.ToInt32(x!.ToString()));
        var cellScores = list["CellScore"]!.AsArray().Select(x => Convert.ToDecimal(x!.ToString()));
        var allBets = list["AllBet"]!.AsArray().Select(x => Convert.ToDecimal(x!.ToString()));
        var profits = list["Profit"]!.AsArray().Select(x => Convert.ToDecimal(x!.ToString()));
        var revenues = list["Revenue"]!.AsArray().Select(x => Convert.ToDecimal(x!.ToString()));
        var gameStartTimes = list["GameStartTime"]!.AsArray().Select(x => x!.ToString().StringConvertDateTimeOffset());
        var gameEndTimes = list["GameEndTime"]!.AsArray().Select(x => x!.ToString().StringConvertDateTimeOffset());
        var cardValues = list["CardValue"]!.AsArray().Select(x => (x is null) ? string.Empty : x!.ToString());
        var channelIDs = list["ChannelID"]!.AsArray().Select(x => Convert.ToInt32(x!.ToString()));
        var lineCodes = list["LineCode"]!.AsArray().Select(x => x!.ToString());

        foreach (var it in gameIds.Select((e, i) => new { Value = e, Index = i }))
        {
            yield return new GameRecordInfo(
                GameID: it.Value,
                Account: accounts.ElementAt(it.Index).Replace($"{agentId}_", string.Empty),
                ServerID: serverIds.ElementAt(it.Index),
                KindID: kindIds.ElementAt(it.Index),
                TableID: tableIds.ElementAt(it.Index),
                ChairID: chairIds.ElementAt(it.Index),
                UserCount: userCount.ElementAt(it.Index),
                CellScore: cellScores.ElementAt(it.Index),
                AllBet: allBets.ElementAt(it.Index),
                Profit: profits.ElementAt(it.Index),
                Revenue: revenues.ElementAt(it.Index),
                GameStartTime: gameStartTimes.ElementAt(it.Index),
                GameEndTime: gameEndTimes.ElementAt(it.Index),
                CardValue: cardValues.ElementAt(it.Index),
                ChannelID: channelIDs.ElementAt(it.Index),
                LineCode: lineCodes.ElementAt(it.Index),
                AdditionalData: new JsonObject
                {
                    ["GameID"] = it.Value,
                    ["Accounts"] = accounts.ElementAt(it.Index),
                    ["ServerID"] = serverIds.ElementAt(it.Index),
                    ["KindID"] = kindIds.ElementAt(it.Index),
                    ["TableID"] = tableIds.ElementAt(it.Index),
                    ["ChairID"] = chairIds.ElementAt(it.Index),
                    ["UserCount"] = userCount.ElementAt(it.Index),
                    ["CellScore"] = cellScores.ElementAt(it.Index),
                    ["AllBet"] = allBets.ElementAt(it.Index),
                    ["Profit"] = profits.ElementAt(it.Index),
                    ["Revenue"] = revenues.ElementAt(it.Index),
                    ["GameStartTime"] = gameStartTimes.ElementAt(it.Index),
                    ["GameEndTime"] = gameEndTimes.ElementAt(it.Index),
                    ["CardValue"] = cardValues.ElementAt(it.Index),
                    ["ChannelID"] = channelIDs.ElementAt(it.Index),
                    ["LineCode"] = lineCodes.ElementAt(it.Index)
                }.ToJsonString());
        }
    }

    private async ValueTask<JsonNode> GetAsync(Uri apiUrl, string logPrefix, string deskey, string md5key, string agentId, string data, CancellationToken cancellationToken = default)
    {
        var timestamp = _systemClock.CurrentTime.ToUnixTimeMilliseconds();
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/getRecordHandle"));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["agent"] = agentId;
        query["timestamp"] = timestamp.ToString();
        query["param"] = await _cryptography
            .GetAesEncryptAsync(
            data: data,
            secretKey: deskey,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        query["key"] = _cryptography.GetHash($"{agentId}{timestamp}{md5key}");
        uriBuilder.Query = query.ToString();

        logPrefix = $"{nameof(GameApiClient)}.{logPrefix}";

        var sw = Stopwatch.StartNew();

        var httpResponseMessage = await _client
            .GetAsync(
                requestUri: uriBuilder.ToString(),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        sw.Stop();

        var responseMessage = default(string);
        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
        {
            responseMessage = await sr
                .ReadToEndAsync(
                    cancellationToken: cancellationToken)
                ?? string.Empty;
        }

        var logEvent = new GameApiLogEvent
        {
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = data.NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _logger.LogInformation("{logEvent}", JsonSerializer.Serialize(logEvent));

        _ = httpResponseMessage.EnsureSuccessStatusCode();

        return string.IsNullOrEmpty(responseMessage)
            ? throw new Exception("Game provider response content is empty.")
            : JsonNode.Parse(responseMessage)!;
    }
}
