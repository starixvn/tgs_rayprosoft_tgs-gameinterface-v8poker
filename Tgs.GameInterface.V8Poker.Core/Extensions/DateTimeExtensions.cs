namespace Tgs.GameInterface.V8Poker.Core.Extensions;

public static class DateTimeExtensions
{
	private static readonly TimeZoneInfo _TimeZone;

	static DateTimeExtensions()
	{
		_TimeZone = TimeZoneInfo.CreateCustomTimeZone(id: "Taipei Standard Time", baseUtcOffset: TimeSpan.FromHours(+8), "Taipei Standard Time", "Taipei Standard Time");
	}

	public static DateTimeOffset StringConvertDateTimeOffset(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return DateTime.UnixEpoch;
		}

		var date = DateTime.Parse(str);

		return new DateTimeOffset(date, _TimeZone.GetUtcOffset(date));
	}
}
