using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.V8Poker.Core.ApplicationServices;
using Tgs.GameInterface.V8Poker.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.V8Poker.GrpcServices;

public class GameInterfaceV8PokerGrpcService : GameInterfaceV8Poker.GameInterfaceV8PokerBase
{
    private readonly IGetService<FundInQuery, FundResult> _fundInService;
    private readonly IGetService<FundOutQuery, FundResult> _fundOutService;
    private readonly IGetService<FundStatusQuery, bool> _fundStatusService;
    private readonly IQueryService<GameWagerQuery, GameWagerResult> _gameWagerService;
    private readonly IGetService<GameUrlQuery, string> _gameUrlService;
    private readonly ILogger<GameInterfaceV8PokerGrpcService> _logger;
    private readonly IGetService<PlayerBalanceQuery, PlayerBalanceResult> _playerBalanceService;
    private readonly ICommandService<PlayerLogoutCommand> _playerLogoutService;
    private readonly IGetService<AgentBalanceQuery, AgentBalanceResult> _agentBalanceService;

    public GameInterfaceV8PokerGrpcService(
        ILogger<GameInterfaceV8PokerGrpcService> logger,
        IGetService<FundInQuery, FundResult> fundInService,
        IGetService<FundOutQuery, FundResult> fundOutService,
        IGetService<FundStatusQuery, bool> fundStatusService,
        IQueryService<GameWagerQuery, GameWagerResult> gameWagerService,
        IGetService<GameUrlQuery, string> gameUrlService,
        IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
        ICommandService<PlayerLogoutCommand> playerLogoutService,
        IGetService<AgentBalanceQuery, AgentBalanceResult> agentBalanceService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
        _fundStatusService = fundStatusService ?? throw new ArgumentNullException(nameof(fundStatusService));
        _gameWagerService = gameWagerService ?? throw new ArgumentNullException(nameof(gameWagerService));
        _gameUrlService = gameUrlService ?? throw new ArgumentNullException(nameof(gameUrlService));
        _playerBalanceService = playerBalanceService ?? throw new ArgumentNullException(nameof(playerBalanceService));
        _playerLogoutService = playerLogoutService ?? throw new ArgumentNullException(nameof(playerLogoutService));
        _agentBalanceService = agentBalanceService ?? throw new ArgumentNullException(nameof(agentBalanceService));
    }

    public override async Task<AgentBalanceReply> AgentBalance(AgentBalanceRequest request, ServerCallContext context)
    {
        var query = new AgentBalanceQuery(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId);

        var result = await _agentBalanceService
            .GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new AgentBalanceReply
        {
            Credit = result.Credit
        };
    }

    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        _logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

        return Task.FromResult(new CheckingServiceReply
        {
            MachineName = Environment.MachineName
        });
    }

    public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
    {
        var query = new FundInQuery(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId,
            PlayerCode: request.PlayerCode,
            OrderId: request.OrderId,
            Amount: request.Amount);

        var result = await _fundInService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            Resp = result.Resp,
        };
    }

    public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
    {
        var query = new FundOutQuery(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId,
            PlayerCode: request.PlayerCode,
            OrderId: request.OrderId,
            Amount: request.Amount);

        var result = await _fundOutService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            Resp = result.Resp,
        };
    }

    public override async Task<BaseReply> FundStatus(FundStatusRequest request, ServerCallContext context)
    {
        var query = new FundStatusQuery(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId,
            OrderId: request.OrderId);

        var state = await _fundStatusService
            .GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = state
        };
    }

    public override async Task<GameWagerReply> GameWager(GameWagerRequest request, ServerCallContext context)
    {
        var query = new GameWagerQuery(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId,
            StartTime: request.StartTime.ToDateTimeOffset(),
            EndTime: request.EndTime.ToDateTimeOffset());

        var results = await _gameWagerService
            .QueryAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ToArrayAsync(
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        var reply = new GameWagerReply();

        if (results.Any())
        {
            foreach (var result in results)
            {
                reply.Items.Add(new GameWagerInfo
                {
                    ExternalWagerId = result.ExternalWagerId,
                    ExternalGameId = result.ExternalGameId,
                    PlayerCode = result.PlayerCode,
                    ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
                    ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
                    Bet = result.Bet,
                    ValidBet = result.ValidBet,
                    Settle = result.Settle,
                    Commission = result.Commission,
                    WinLoss = result.WinLoss,
                    AdditionalData = result.AdditionalData
                });
            }
        }

        return reply;
    }

    public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
    {
        var query = new GameUrlQuery(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId,
            BrandCode: request.BrandCode,
            PlayerCode: request.PlayerCode,
            IpAddress: request.IpAddress,
            ExternalGameId: request.ExternalGameId,
            Lang: request.Lang);

        var result = await _gameUrlService
            .GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new GameUrlReply
        {
            GameUrl = result
        };
    }

    public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId,
            PlayerCode: request.PlayerCode);

        var result = await _playerBalanceService
            .GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new PlayerBalanceReply
        {
            Balance = result.Balance,
            FreeBalance = result.FreeBalance,
            OutstandingBalance = result.OutstandingBalance,
        };
    }

    public override async Task<BaseReply> PlayerLogout(PlayerLogoutRequest request, ServerCallContext context)
    {
        var command = new PlayerLogoutCommand(
            ApiUrl: new(request.ApiUrl),
            Deskey: request.Deskey,
            MD5key: request.MD5Key,
            AgentId: request.AgentId,
            PlayerCode: request.PlayerCode);

        await _playerLogoutService
            .ExecuteAsync(
            command: command,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = true
        };
    }
}
