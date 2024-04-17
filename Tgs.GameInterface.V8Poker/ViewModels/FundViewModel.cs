using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.V8Poker.ViewModels;

public record FundViewModel
{
	[Required]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string Deskey { get; init; } = default!;
	[Required]
	public string MD5key { get; init; } = default!;
	[Required]
	public string AgentId { get; init; } = default!;
	[Required]
	public string PlayerCode { get; init; } = default!;
	[Required]
	public string OrderId { get; init; } = default!;
	[Required]
	public decimal Amount { get; init; }
}
