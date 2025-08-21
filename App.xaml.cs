using System;
using System.Windows;


namespace WinTodoNag
{
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      // Single instance
      if (!Services.SingleInstanceService.TryClaim("WinTodoNag"))
      {
        MessageBox.Show("WinTodoNag is already running.");
        Shutdown();
        return;
      }


      // Ensure tray starts
      Services.TrayService.Initialize();
    }


    protected override void OnExit(ExitEventArgs e)
    {
      Services.TrayService.Dispose();
      base.OnExit(e);
    }
  }
}