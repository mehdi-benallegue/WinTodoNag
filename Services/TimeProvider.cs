// PATCH: replace file contents
using System;

namespace WinTodoNag.Services
{
  public static class TimeProvider
  {
    public static Func<DateTimeOffset> Now = () => DateTimeOffset.Now;
    public static TimeZoneInfo LocalTimeZone => TimeZoneInfo.Local;
  }
}