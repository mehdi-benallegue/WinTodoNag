using System.Windows.Input;
using Microsoft.Win32;
using WinTodoNag.Services;
using WinTodoNag.Utils;


namespace WinTodoNag.ViewModels
{
  public class SettingsViewModel
  {
    public string DataFilePath { get => StorageService.FilePath; set { StorageService.SetFilePath(value); } }
    public bool RunAtStartup { get => Services.AutoStartService.IsEnabled(); set { Services.AutoStartService.SetEnabled(value); } }
    public bool MinimizeToTray { get => Services.TrayService.MinimizeToTray; set { Services.TrayService.MinimizeToTray = value; } }
    public bool AllowQuitFromTray { get => Services.TrayService.AllowQuitFromTray; set { Services.TrayService.AllowQuitFromTray = value; } }


    public ICommand BrowseDataFileCommand { get; }
    public SettingsViewModel()
    {
      BrowseDataFileCommand = new RelayCommand(_ =>
      {
        var dlg = new SaveFileDialog { Filter = "YAML (*.yaml)|*.yaml", FileName = StorageService.FilePath };
        if (dlg.ShowDialog() == true) StorageService.SetFilePath(dlg.FileName);
      });
    }
  }
}