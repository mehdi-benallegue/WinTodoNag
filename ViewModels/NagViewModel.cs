using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WinTodoNag.Models;
using WinTodoNag.Services;
using WinTodoNag.Utils;

namespace WinTodoNag.ViewModels
{
  public class NagViewModel
  {
    private readonly TaskItem _task;
    public string Title => _task.Title;
    public string Notes => _task.Notes;

    public ObservableCollection<string> SnoozeChoices { get; } = new()
    { "2m","5m","10m","15m","1h","2h","4h","8h","1d","2d","4d","1w","2w","3w","1mo","2mo","3mo" };

    public string SelectedSnooze { get; set; } = "10m";

    public string CountdownText => "Due now";

    public string PreviewNextText { get; private set; } = string.Empty;
    public System.Windows.Visibility PreviewNextVisible =>
      string.IsNullOrEmpty(PreviewNextText) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

    public ICommand SnoozeCommand { get; }
    public ICommand CustomSnoozeCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand OpenTaskCommand { get; }

    public NagViewModel(TaskItem task)
    {
      _task = task;

      SnoozeCommand = new RelayCommand(_ =>
      {
        var now = WinTodoNag.Services.TimeProvider.Now();
        var until = DateTimeExtensions.ApplyPreset(now, SelectedSnooze);
        if (until <= now)
        {
          System.Windows.MessageBox.Show("Snooze target must be in the future.");
          return;
        }
        PreviewNextText = $"Will ring again in {DateTimeExtensions.HumanDiff(until)} on {until:yyyy-MM-dd HH:mm}";
        SchedulerService.Reschedule(_task, until); // persists and reorders
      });

      CustomSnoozeCommand = new RelayCommand(_ =>
      {
        // TODO: open a custom datetime picker dialog; placeholder for now
      });

      CompleteCommand = new RelayCommand(_ =>
      {
        _task.MarkCompletedRecursive(manual: true);
        StorageService.Save();
        Services.NagService.CloseActive();
      });

      OpenTaskCommand = new RelayCommand(_ =>
      {
        // TODO: open task editor dialog focusing this task
      });
    }
  }
}
