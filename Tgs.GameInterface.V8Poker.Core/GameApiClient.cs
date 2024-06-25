using System.Diagnostics;
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

internal class GameApiClient(
	ILogger<GameApiClient> logger,
	IOptions<PolicySettingsOptions> options,
	IHashCryptography cryptography,
	ISystemClock systemClock,
	HttpClient client) : IGameApiClient
{
	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async ValueTask<BalanceInfo> BalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, string account, CancellationToken cancellationToken = default)
	{
		var data = $"s=7&account={account}";
		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			logPrefix: nameof(BalanceAsync),
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;
		return dataResponse["code"]!.GetValue<int>() != 0
			? throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>())
			: new BalanceInfo(
			dataResponse["totalMoney"]!.GetValue<decimal>(),
			dataResponse["freeMoney"]!.GetValue<decimal>());
	}

	public async ValueTask<DepositResp> DepositAsync(Uri apiUrl, string deskey, string md5key, string agentId, string account, string orderId, decimal money, CancellationToken cancellationToken = default)
	{
		var data = $"s=2&account={account}&money={money}&orderid={orderId}";

		var retryPolicy = Policy
			.HandleResult<JsonNode>(jsonNode => jsonNode["d"]!["code"]!.GetValue<int>() == 39)
			.WaitAndRetryAsync(
			retryCount: _policySettingsOptions.DepositRetryCount,
			retryAttempt => TimeSpan.FromSeconds(_policySettingsOptions.DepositRetryWaitSec),
			onRetry: (response, retryTime, context) => logger.LogWarning("LogOn:{logOn} | Message:禁止同一帳號登入帶分、上分、下分並發請求，後一個請求被拒 | Json:{jsonNode}", DateTime.Now.ToString("o"), response.Result.ToJsonString()));

		var jsonNode = await retryPolicy
			.ExecuteAsync(
			async () => await GetAsync(
				apiUrl: apiUrl,
				logPrefix: nameof(DepositAsync),
				deskey: deskey,
				md5key: md5key,
				agentId: agentId,
				data: data,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;

		return dataResponse["code"]!.GetValue<int>() != 0
			? throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>())
			: new DepositResp(jsonNode.ToJsonString());
	}

	public async ValueTask<string> LoginAsync(Uri apiUrl, string deskey, string md5key, string agentId, string account, string ipAddress, string lineCode, string kindId, CancellationToken cancellationToken = default)
	{
		var orderId = $"{agentId}{systemClock.CurrentTime:yyyyMMddHHmmssfff}{account}";
		var data = $"s=0&account={account}&money=0&orderid={orderId}&ip={ipAddress}&lineCode={lineCode}&KindID={kindId}";
		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			logPrefix: nameof(LoginAsync),
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;
		return dataResponse["code"]!.GetValue<int>() != 0
			? throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>())
			: dataResponse["url"]!.GetValue<string>();
	}

	public async ValueTask LogoutAsync(Uri apiUrl, string deskey, string md5key, string agentId, string account, CancellationToken cancellationToken = default)
	{
		var data = $"s=8&account={account}";
		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			logPrefix: nameof(LogoutAsync),
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;
		if (dataResponse["code"]!.GetValue<int>() != 0)
		{
			throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>());
		}
	}

	public async ValueTask<bool> GetOrderAsync(Uri apiUrl, string deskey, string md5key, string agentId, string orderId, CancellationToken cancellationToken = default)
	{
		var data = $"s=4&&orderid={orderId}";
		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			logPrefix: nameof(GetOrderAsync),
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;
		return dataResponse["code"]!.GetValue<int>() != 0
			? throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>())
			: dataResponse["status"]!.GetValue<int>() is not (-1) and not 0 and not 2
				? throw new Exception($"訂單狀態請聯絡客服協助處理(status:{dataResponse["status"]!.GetValue<int>()})")
				: dataResponse["status"]!.GetValue<int>() == 0;
	}

	public async ValueTask<WithdrawResp> WithdrawAsync(Uri apiUrl, string deskey, string md5key, string agentId, string account, string orderId, decimal money, CancellationToken cancellationToken = default)
	{
		var data = $"s=3&account={account}&money={money}&orderid={orderId}";
		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			logPrefix: nameof(WithdrawAsync),
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;

		return dataResponse["code"]!.GetValue<int>() != 0
			? throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>())
			: new WithdrawResp(jsonNode.ToJsonString());
	}

	public async ValueTask<decimal> AgentBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, CancellationToken cancellationToken = default)
	{
		var data = $"s=14";
		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			logPrefix: nameof(AgentBalanceAsync),
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;

		return dataResponse["code"]!.GetValue<int>() != 0
			? throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>())
			: decimal.Parse(jsonNode["d"]!["money"]!.ToString());
	}

	public async ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, string deskey, string md5key, string agentId, string account, CancellationToken cancellationToken = default)
	{
		var data = $"s=5&account={account}";
		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			logPrefix: nameof(PlayerOnlineAsync),
			deskey: deskey,
			md5key: md5key,
			agentId: agentId,
			data: data,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var dataResponse = jsonNode["d"]!;
		return dataResponse["code"]!.GetValue<int>() != 0
			? throw new ReturnCodeException(dataResponse["code"]!.GetValue<int>())
			: dataResponse["status"]!.GetValue<int>() == 1;
	}

	private async ValueTask<JsonNode> GetAsync(Uri apiUrl, string logPrefix, string deskey, string md5key, string agentId, string data, CancellationToken cancellationToken = default)
	{
		var timestamp = systemClock.CurrentTime.ToUnixTimeMilliseconds();
		var uriBuilder = new UriBuilder(new Uri(apiUrl, "/channelHandle"));
		var query = HttpUtility.ParseQueryString(uriBuilder.Query);
		query["agent"] = agentId;
		query["timestamp"] = timestamp.ToString();
		query["param"] = await cryptography
			.GetAesEncryptAsync(
			data: data,
			secretKey: deskey,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		query["key"] = cryptography.GetHash($"{agentId}{timestamp}{md5key}");
		uriBuilder.Query = query.ToString();

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await client
			.GetAsync(
				requestUri: uriBuilder.ToString(),
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);
		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync(cancellationToken: cancellationToken).ConfigureAwait(false) ?? string.Empty;
		}

		var logEvent = new
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

		logger.LogInformation("{logEvent}", JsonSerializer.Serialize(logEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return string.IsNullOrEmpty(responseMessage)
			? throw new Exception("Game provider response content is empty.")
			: JsonNode.Parse(responseMessage)!;
	}
}
