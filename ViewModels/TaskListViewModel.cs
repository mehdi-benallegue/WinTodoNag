using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WinTodoNag.Models;
using WinTodoNag.Services;
using WinTodoNag.Utils;
using WinTodoNag.Views;


namespace WinTodoNag.ViewModels
{
  public class TaskListViewModel
  {
    public ObservableCollection<TaskItem> Tasks { get; } = new();


    public ICommand NewTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand SnoozeTaskCommand { get; }


    public TaskListViewModel()
    {
      // Bind to storage
      foreach (var t in StorageService.Current.Tasks.OrderBy(t => t.NextNotificationAt))
        Tasks.Add(t);


      NewTaskCommand = new RelayCommand(_ =>
      {
        var vm = new TaskEditorViewModel();
        var dlg = new TaskEditorDialog { DataContext = vm };
        if (dlg.ShowDialog() == true)
        {
          var task = vm.ToTaskItem();
          StorageService.Current.Tasks.Add(task);
          Tasks.Add(task);
          StorageService.Save();
        }
      });


      EditTaskCommand = new RelayCommand(obj =>
      {
        if (obj is not TaskItem t) return;
        var vm = TaskEditorViewModel.FromTask(t);
        var dlg = new TaskEditorDialog { DataContext = vm };
        if (dlg.ShowDialog() == true)
        {
          vm.ApplyTo(t);
          StorageService.Save();
        }
      });


      SnoozeTaskCommand = new RelayCommand(obj =>
      {
        if (obj is not TaskItem t) return;
        var until = Utils.DateTimeExtensions.ApplyPreset(DateTime.Now, "10m"); // simple default snooze
        t.NextNotificationAt = until;
        Services.StorageService.Save();
      });
    }
  }
}