using System.Windows.Input;
using WinTodoNag.Utils;
using WinTodoNag.Views;
using WinTodoNag.Services;


namespace WinTodoNag.ViewModels
{
  public class MainViewModel
  {
    public TaskListViewModel TaskListVM { get; } = new();
    public CalendarViewModel CalendarVM { get; } = new();
    public SettingsViewModel SettingsVM { get; } = new();


    public ICommand OpenSettingsCommand { get; }
    public ICommand OpenDataFileCommand { get; }


    public MainViewModel()
    {
      OpenSettingsCommand = new RelayCommand(_ =>
      {
        var w = new SettingsView { DataContext = SettingsVM };
        var win = new Window { Title = "Settings", Content = w, Width = 520, Height = 260, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner };
        win.ShowDialog();
      });


      OpenDataFileCommand = new RelayCommand(_ => StorageService.RevealDataFile());


      // Start scheduler after loading data
      StorageService.LoadOrInit();
      SchedulerService.Initialize(TaskListVM.Tasks);
    }
  }
}