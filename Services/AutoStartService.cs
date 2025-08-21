using Microsoft.Win32;


namespace WinTodoNag.Services
{
  public static class AutoStartService
  {
    private const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    private const string AppName = "WinTodoNag";


    public static void SetEnabled(bool enabled)
    {
      using var key = Registry.CurrentUser.OpenSubKey(RunKey, true)!;
      if (enabled) key.SetValue(AppName, System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName);
      else key.DeleteValue(AppName, false);
    }


    public static bool IsEnabled()
    {
      using var key = Registry.CurrentUser.OpenSubKey(RunKey, false)!;
      return key.GetValue(AppName) != null;
    }
  }
}