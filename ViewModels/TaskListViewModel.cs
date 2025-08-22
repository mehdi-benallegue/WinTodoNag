using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WinTodoNag.Models;
using WinTodoNag.Services;
using WinTodoNag.Utils;
using WinTodoNag.Views;
using TP = WinTodoNag.Services.TimeProvider; // << alias

namespace WinTodoNag.ViewModels
{
  public class TaskListViewModel
  {
    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ICollectionView View { get; }

    public ICommand NewTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand SnoozeTaskCommand { get; }

    public TaskListViewModel()
    {
      foreach (var t in StorageService.Current.Tasks)
        Tasks.Add(t);

      View = CollectionViewSource.GetDefaultView(Tasks);
      View.SortDescriptions.Add(new SortDescription(nameof(TaskItem.NextNotificationAt), ListSortDirection.Ascending));
      View.SortDescriptions.Add(new SortDescription(nameof(TaskItem.DeadlineAt), ListSortDirection.Ascending));
      View.SortDescriptions.Add(new SortDescription(nameof(TaskItem.CreatedAt), ListSortDirection.Ascending));

      StorageService.DataChanged += () =>
      {
        Tasks.Clear();
        foreach (var t in StorageService.Current.Tasks)
          Tasks.Add(t);
        View.Refresh();
      };

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
          View.Refresh();
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
          View.Refresh();
        }
      });

      SnoozeTaskCommand = new RelayCommand(obj =>
      {
        if (obj is not TaskItem t) return;
        var until = DateTimeExtensions.ApplyPreset(TP.Now(), "10m");
        SchedulerService.Reschedule(t, until);
        View.Refresh();
      });
    }
  }
}
