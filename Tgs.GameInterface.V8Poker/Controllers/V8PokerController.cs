using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;
using Tgs.GameInterface.V8Poker.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.Controllers;

[Route("[controller]")]
[ApiController]
public class V8PokerController : ControllerBase
{
    /// <summary>
    /// Funds the in asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "playerCode":"4uh3pdl0lcii",
    ///             "orderId": "60073202310120338068944uh3pdl0lcii"
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/in", Name = "FundIn")]
    public async ValueTask<FundResultViewModel> FundInAsync(
        [FromServices] IGetService<FundInQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundInQuery(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId,
            PlayerCode: source.PlayerCode,
            OrderId: source.OrderId,
            Amount: source.Amount);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new FundResultViewModel(result.Resp);
    }

    /// <summary>
    /// Funds the out asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "playerCode":"4uh3pdl0lcii",
    ///             "orderId": "6007320202310120336009404uh3pdl0lcii",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/out", Name = "FundOut")]
    public async ValueTask<FundResultViewModel> FundOutAsync(
        [FromServices] IGetService<FundOutQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundOutQuery(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId,
            PlayerCode: source.PlayerCode,
            OrderId: source.OrderId,
            Amount: source.Amount);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new FundResultViewModel(result.Resp);
    }

    /// <summary>
    /// Funds the status asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "orderId": "60073202310111727221424uh3pdl0lcii"
    ///         }
    /// </remarks>
    [HttpPost("fund/Status", Name = "FundStatus")]
    public ValueTask<bool> FundStatusAsync(
        [FromServices] IGetService<FundStatusQuery, bool> service,
        FundStatusViewModel source)
    {
        var query = new FundStatusQuery(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId,
            OrderId: source.OrderId);

        return service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Games the record asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://wc-record.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "startTime": "2023-07-26T11:50:00+08:00",
    ///             "endTime": "2023-07-26T12:00:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost("game/record")]
    public async IAsyncEnumerable<GameRecordResultViewModel> GameRecordAsync(
        [FromServices] IQueryService<GameWagerQuery, GameWagerResult> service,
        GameRecordViewModel source)
    {
        var query = new GameWagerQuery(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId,
            StartTime: source.StartTime,
            EndTime: source.EndTime);

        var results = await service
            .QueryAsync(
                query: query,
                cancellationToken: HttpContext.RequestAborted)
            .ToArrayAsync(
                cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        if (!results.Any())
        {
            yield break;
        }

        foreach (var result in results)
        {
            yield return new GameRecordResultViewModel(
                ExternalWagerId: result.ExternalWagerId,
                ExternalGameId: result.ExternalGameId,
                PlayerCode: result.PlayerCode,
                ExternalCreatedOn: result.ExternalCreatedOn,
                ExternalClosedOn: result.ExternalClosedOn,
                Bet: result.Bet,
                ValidBet: result.ValidBet,
                Settle: result.Settle,
                Commission: result.Commission,
                WinLoss: result.WinLoss,
                AdditionalData: result.AdditionalData);
        }
    }

    /// <summary>
    /// Games the URL asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "brandCode": "GPI_PHP",
    ///             "playerCode":"4uh3pdl0lcii",
    ///             "ipAddress": "127.0.0.1",
    ///             "externalGameId": "0",
    ///             "lang": "en-us"
    ///         }
    ///
    ///
    ///     Post Baccarat
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "brandCode": "GPI_PHP",
    ///             "playerCode":"4uh3pdl0lcii",
    ///             "ipAddress": "127.0.0.1",
    ///             "externalGameId": "910",
    ///             "lang": "zh-cn"
    ///         }
    /// </remarks>
    [HttpPost("gameurl")]
    public ValueTask<string> GameUrlAsync(
        [FromServices] IGetService<GameUrlQuery, string> service,
        GameUrlViewModel source)
    {
        var query = new GameUrlQuery(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId,
            BrandCode: source.BrandCode,
            PlayerCode: source.PlayerCode,
            IpAddress: source.IpAddress,
            ExternalGameId: source.ExternalGameId,
            Lang: source.Lang);

        return service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Players the balance asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "playerCode":"4uh3pdl0lcii"
    ///         }
    /// </remarks>
    [HttpPost("player/balance", Name = "PlayerBalance")]
    public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
        [FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
        PlayerBalanceViewModel source)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId,
            PlayerCode: source.PlayerCode);

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new PlayerBalanceResultViewModel(
            Balance: result.Balance,
            FreeBalance: result.FreeBalance,
            OutstandingBalance: result.OutstandingBalance);
    }

    /// <summary>
    /// Players the logout asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073",
    ///             "playerCode":"4uh3pdl0lcii"
    ///         }
    /// </remarks>
    [HttpPost("player/logout", Name = "PlayerLogout")]
    public ValueTask PlayerLogoutAsync(
        [FromServices] ICommandService<PlayerLogoutCommand> service,
        PlayerLogoutViewModel source)
    {
        var command = new PlayerLogoutCommand(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId,
            PlayerCode: source.PlayerCode);

        return service
            .ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Players the balance asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": " https://wc-api.orz16.com/",
    ///             "deskey": "03CFE0179704BCDF",
    ///             "mD5key": "337B01054C6FA208",
    ///             "agentId": "60073"
    ///         }
    /// </remarks>
    [HttpPost("agent/balance", Name = "AgentBalance")]
    public async ValueTask<AgentBalanceResultViewModel> AgentBalanceAsync(
        [FromServices] IGetService<AgentBalanceQuery, AgentBalanceResult> service,
        AgentBalanceViewModel source)
    {
        var query = new AgentBalanceQuery(
            ApiUrl: source.ApiUrl,
            Deskey: source.Deskey,
            MD5key: source.MD5key,
            AgentId: source.AgentId);

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new AgentBalanceResultViewModel(
            Credit: result.Credit);
    }
}
