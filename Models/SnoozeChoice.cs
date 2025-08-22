using System;
using TP = WinTodoNag.Services.TimeProvider; // << alias

namespace WinTodoNag.Models
{
  public static class SnoozeChoice
  {
    public static DateTimeOffset Minutes(int m) => TP.Now().AddMinutes(m);
    public static DateTimeOffset Hours(int h) => TP.Now().AddHours(h);
    public static DateTimeOffset Days(int d) => TP.Now().AddDays(d);
    public static DateTimeOffset Weeks(int w) => TP.Now().AddDays(7 * w);
    public static DateTimeOffset Months(int m) => TP.Now().AddMonths(m);
  }
}
