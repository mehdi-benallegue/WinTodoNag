using System;


namespace WinTodoNag.Utils
{
  public static class DateTimeExtensions
  {
    public static DateTime StartOfMonth(this DateTime dt) => new DateTime(dt.Year, dt.Month, 1);
    public static DateTime CombineTime(this DateTime date, string hhmm)
    {
      if (TimeSpan.TryParse(hhmm, out var t)) return date.Date + t;
      return date.Date + new TimeSpan(9, 0, 0);
    }


    public static DateTime ApplyPreset(DateTime from, string preset)
    {
      return preset switch
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
    }


    public static string HumanDiff(DateTime to)
    {
      var span = to - DateTime.Now;
      return span.ToString();
    }
  }
}