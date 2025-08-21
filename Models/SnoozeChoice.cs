using System;


namespace WinTodoNag.Models
{
  public static class SnoozeChoice
  {
    public static DateTime Minutes(int m) => DateTime.Now.AddMinutes(m);
    public static DateTime Hours(int h) => DateTime.Now.AddHours(h);
    public static DateTime Days(int d) => DateTime.Now.AddDays(d);
    public static DateTime Weeks(int w) => DateTime.Now.AddDays(7 * w);
    public static DateTime Months(int m) => DateTime.Now.AddMonths(m);
  }
}