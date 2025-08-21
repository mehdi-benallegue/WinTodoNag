using System;
using System.Windows.Forms;


namespace WinTodoNag.Services
{
  public static class TrayService
  {
    private static NotifyIcon? _ni;
    public static bool MinimizeToTray { get; set; } = true;
    public static bool AllowQuitFromTray { get; set; } = false;


    public static void Initialize()
    {
      _ni = EnsureNotifyIcon();
    }


    public static NotifyIcon EnsureNotifyIcon()
    {
      if (_ni != null) return _ni;
      _ni = new NotifyIcon { Visible = true, Text = "WinTodoNag" };
      _ni.Icon = System.Drawing.SystemIcons.Application;
      var menu = new ContextMenuStrip();
      var open = new ToolStripMenuItem("Open"); open.Click += (_, __) => ShowMain();
      menu.Items.Add(open);
      if (AllowQuitFromTray)
      {
        var quit = new ToolStripMenuItem("Quit"); quit.Click += (_, __) => System.Windows.Application.Current.Shutdown();
        menu.Items.Add(quit);
      }
      _ni.ContextMenuStrip = menu;
      _ni.DoubleClick += (_, __) => ShowMain();
      return _ni;
    }


    public static void Dispose()
    {
      if (_ni != null) { _ni.Visible = false; _ni.Dispose(); _ni = null; }
    }


    private static void ShowMain()
    {
      foreach (var w in System.Windows.Application.Current.Windows)
      {
        if (w is MainWindow mw)
        {
          mw.Show(); mw.WindowState = System.Windows.WindowState.Normal; mw.Activate();
          return;
        }
      }
      new MainWindow().Show();
    }
  }
}