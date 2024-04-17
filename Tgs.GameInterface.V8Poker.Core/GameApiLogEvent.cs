namespace Tgs.GameInterface.V8Poker.Core;

public record GameApiLogEvent
{
	public int ExecutionTimeMs { get; set; }

	public string Method { get; set; } = default!;

	public string Path { get; set; } = default!;

	public string Prefix { get; set; } = default!;

	public DateTimeOffset RecordedOn { get; set; }

	public string RequestContent { get; set; } = default!;

	public string RequestHeaders { get; set; } = default!;

	public short ResponseCode { get; set; }

	public string ResponseContent { get; set; } = default!;

	public string ResponseHeaders { get; set; } = default!;

	public string ServerName { get; set; } = default!;
}
