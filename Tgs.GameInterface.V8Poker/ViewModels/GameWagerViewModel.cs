using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.V8Poker.ViewModels;

public record GameWagerViewModel
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
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset EndTime { get; init; } = default!;
}
