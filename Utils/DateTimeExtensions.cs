using System;
using TP = WinTodoNag.Services.TimeProvider; // << alias

namespace WinTodoNag.Utils
{
  public static class DateTimeExtensions
  {
    public static DateTimeOffset StartOfMonth(this DateTimeOffset dt)
      => new DateTimeOffset(dt.Year, dt.Month, 1, 0, 0, 0, dt.Offset);

    public static DateTimeOffset CombineTime(this DateTimeOffset date, string hhmm)
    {
      // Parse "HH:mm" safely; default to 09:00 if parsing fails
      if (!TimeSpan.TryParse(hhmm, out var t))
        t = new TimeSpan(9, 0, 0);

      // IMPORTANT: construct a DateTimeOffset with the SAME calendar date and SAME offset,
      // without adding the offset as a duration.
      return new DateTimeOffset(
        date.Year, date.Month, date.Day,
        t.Hours, t.Minutes, t.Seconds,
        date.Offset
      );
    }

    public static DateTimeOffset ApplyPreset(DateTimeOffset from, string preset) => preset switch
    {
      "2m" => from.AddMinutes(2),
      "5m" => from.AddMinutes(5),
      "10m" => from.AddMinutes(10),
      "15m" => from.AddMinutes(15),
      "1h" => from.AddHours(1),
      "2h" => from.AddHours(2),
      "4h" => from.AddHours(4),
      "8h" => from.AddHours(8),
      "1d" => from.AddDays(1),
      "2d" => from.AddDays(2),
      "4d" => from.AddDays(4),
      "1w" => from.AddDays(7),
      "2w" => from.AddDays(14),
      "3w" => from.AddDays(21),
      "1mo" => from.AddMonths(1),
      "2mo" => from.AddMonths(2),
      "3mo" => from.AddMonths(3),
      _ => from.AddMinutes(10)
    };

    public static string HumanDiff(DateTimeOffset to)
    {
      var span = to - TP.Now();
      if (span.TotalSeconds < 0) span = -span;
      var d = (int)span.TotalDays;
      var h = span.Hours;
      var m = span.Minutes;
      if (d > 0) return $"{d}d {h}h {m}m";
      if (h > 0) return $"{h}h {m}m";
      return $"{m}m";
    }

    public static DateTime StartOfMonth(this DateTime dt)
  => new DateTime(dt.Year, dt.Month, 1);

  }
}
