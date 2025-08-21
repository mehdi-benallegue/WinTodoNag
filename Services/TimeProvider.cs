using System;


namespace WinTodoNag.Services
{
  public static class TimeProvider
  {
    public static Func<DateTime> Now = () => DateTime.Now;
  }
}