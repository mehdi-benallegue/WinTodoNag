using System;
using System.Windows;
using WinTodoNag.Models;
using WinTodoNag.ViewModels;
using WinTodoNag.Views;


namespace WinTodoNag.Services
{
  public static class NagService
  {
    private static Window? _active;
    public static void ShowNag(TaskItem t)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
              CloseActive();
              var vm = new NagViewModel(t);
        var dlg = new NagDialog { DataContext = vm, Topmost = true, WindowStyle = WindowStyle.ToolWindow };
        _active = dlg;
        dlg.Show(); // Do not Activate(); stays topmost but won’t steal focus
      });
    }


    public static void CloseActive()
    {
      if (_active != null) { _active.Close(); _active = null; }
    }
  }
}