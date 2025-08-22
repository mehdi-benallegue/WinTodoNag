using System;

namespace WinTodoNag
{
  public partial class App : System.Windows.Application
  {
    private static readonly string LogPath = System.IO.Path.Combine(
      AppDomain.CurrentDomain.BaseDirectory, "error.log");

    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
      AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
      {
        var text = "Unhandled exception: " + ex.ExceptionObject + Environment.NewLine;
        SafeAppend(text);
        TryOpenLog();
      };

      this.DispatcherUnhandledException += (s, ex) =>
      {
        var text = "Dispatcher exception: " + ex.Exception + Environment.NewLine;
        SafeAppend(text);
        TryOpenLog();
        System.Windows.MessageBox.Show(ex.Exception.ToString(), "Unhandled UI exception");
        ex.Handled = false;
      };

      try
      {
        base.OnStartup(e);

        if (!Services.SingleInstanceService.TryClaim("WinTodoNag"))
        {
          System.Windows.MessageBox.Show("WinTodoNag is already running.");
          Shutdown();
          return;
        }

        Services.TrayService.Initialize();
        // Keep StartupUri="MainWindow.xaml" in App.xaml (do not show MainWindow here).
      }
      catch (Exception ex)
      {
        SafeAppend(ex + Environment.NewLine);
        TryOpenLog();
        System.Windows.MessageBox.Show(ex.ToString(), "Startup error");
        throw;
      }
    }

    protected override void OnExit(System.Windows.ExitEventArgs e)
    {
      Services.TrayService.Dispose();
      base.OnExit(e);
    }

    private static void SafeAppend(string text)
    {
      try { System.Console.WriteLine(text); } catch { }
      try { System.IO.File.AppendAllText(LogPath, text); } catch { }
    }

    private static void TryOpenLog()
    {
      try
      {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
          FileName = "notepad.exe",
          Arguments = "\"" + LogPath + "\"",
          UseShellExecute = false
        };
        System.Diagnostics.Process.Start(psi);
      }
      catch { /* best-effort */ }
    }
  }
}
