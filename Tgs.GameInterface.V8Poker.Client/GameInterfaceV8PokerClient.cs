using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.V8Poker.Protos;

namespace Tgs.GameInterface.V8Poker.Client;

internal class GameInterfaceV8PokerClient : IGameInterfaceV8PokerClient
{
    private readonly GameInterfaceV8Poker.GameInterfaceV8PokerClient _client;

    public GameInterfaceV8PokerClient(GameInterfaceV8Poker.GameInterfaceV8PokerClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<AgentBalanceReply> AgentBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, CancellationToken cancellationToken = default)
    {
        var request = new AgentBalanceRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId
        };

        return await _client.AgentBalanceAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
    {
        var request = new CheckingServiceRequest
        {
            MachineName = machineName,
        };

        var reply = await _client
            .CheckingServiceAsync(
                request: request,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.MachineName;
    }

    public async ValueTask<FundReply> FundInAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new FundRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId,
            OrderId = orderId,
            PlayerCode = playerCode,
            Amount = amount
        };

        return await _client.FundInAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<FundReply> FundOutAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new FundRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId,
            PlayerCode = playerCode,
            OrderId = orderId,
            Amount = amount
        };

        return await _client.FundOutAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<bool> FundStatusAsync(Uri apiUrl, string deskey, string md5key, string agentId, string orderId, CancellationToken cancellationToken = default)
    {
        var request = new FundStatusRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId,
            OrderId = orderId,
        };

        var reply = await _client.FundStatusAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.State;
    }

    public async IAsyncEnumerable<GameWagerInfo> GameWagerAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset startTime, DateTimeOffset endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId,
            StartTime = startTime.ToTimestamp(),
            EndTime = endTime.ToTimestamp()
        };

        var reply = await _client.GameWagerAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var infos = reply.Items
            ?? Enumerable.Empty<GameWagerInfo>();

        if (!infos.Any())
        {
            yield break;
        }

        foreach (var info in infos)
        {
            yield return info;
        }
    }

    public async ValueTask<string> GameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, string brandCode, string playerCode, string ipAddress, string externalGameId, string lang, CancellationToken cancellationToken = default)
    {
        var request = new GameUrlRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId,
            BrandCode = brandCode,
            PlayerCode = playerCode,
            IpAddress = ipAddress,
            ExternalGameId = externalGameId,
            Lang = lang
        };

        var reply = await _client.GameUrlAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.GameUrl.ToString();
    }

    public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, CancellationToken cancellationToken = default)
    {
        var request = new PlayerBalanceRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId,
            PlayerCode = playerCode
        };

        return await _client.PlayerBalanceAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask PlayerLogoutAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, CancellationToken cancellationToken = default)
    {
        var request = new PlayerLogoutRequest
        {
            ApiUrl = apiUrl.ToString(),
            Deskey = deskey,
            MD5Key = md5key,
            AgentId = agentId,
            PlayerCode = playerCode
        };

        _ = await _client.PlayerLogoutAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
