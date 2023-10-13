using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.V8Poker.ViewModels;

public record AgentBalanceViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string Deskey { get; init; } = default!;
    [Required]
    public string MD5key { get; init; } = default!;
    [Required]
    public string AgentId { get; init; } = default!;
}
