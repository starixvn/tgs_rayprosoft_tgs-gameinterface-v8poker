using System.Web;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Core.ApplicationServices;

public class GameUrlGetService : IGetService<GameUrlQuery, string>
{
    private readonly IGameApiClient _client;

    public GameUrlGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<string> GetAsync(
        GameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var url = await _client.LoginAsync(
            apiUrl: query.ApiUrl,
            deskey: query.Deskey,
            md5key: query.MD5key,
            agentId: query.AgentId,
            account: query.PlayerCode,
            ipAddress: query.IpAddress,
            lineCode: query.BrandCode,
            kindId: query.ExternalGameId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);

        var gameurl = new Uri(url);

        var uriBuilder = new UriBuilder(new Uri(url));
        var queryItems = HttpUtility.ParseQueryString(gameurl.Query);
        queryItems["lang"] = query.Lang;

        uriBuilder.Query = queryItems.ToString();

        return HttpUtility.UrlDecode(uriBuilder.ToString());
    }
}
