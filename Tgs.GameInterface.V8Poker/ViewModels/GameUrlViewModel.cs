using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.V8Poker.ViewModels;

public record GameUrlViewModel
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
    public string BrandCode { get; init; } = default!;
    [Required]
    public string PlayerCode { get; init; } = default!;
    [Required]
    public string IpAddress { get; init; } = default!;
    [Required]
    public string ExternalGameId { get; init; } = default!;
    [Required]
    public string Lang { get; init; } = default!;
}
