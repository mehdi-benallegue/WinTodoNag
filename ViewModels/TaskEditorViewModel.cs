using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WinTodoNag.Models;
using WinTodoNag.Services;
using WinTodoNag.Utils;
using TP = WinTodoNag.Services.TimeProvider;

namespace WinTodoNag.ViewModels
{
  public class TaskEditorViewModel
  {
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // Internal canonical fields (offset-aware)
    public DateTimeOffset? DeadlineDateOffset { get; set; }
    public DateTimeOffset? FirstNotifDateOffset { get; set; } = TP.Now().Date;

    // DatePicker-friendly proxies (DateTime?):
    // We map to local calendar date only; the time comes from the HH:mm string fields.
    public DateTime? DeadlineDateLocal
    {
      get => DeadlineDateOffset?.Date;                      // DateTimeOffset.Date returns DateTime
      set => DeadlineDateOffset = value.HasValue
          ? new DateTimeOffset(value.Value.Date, TP.Now().Offset)
          : (DateTimeOffset?)null;
    }

    public DateTime? FirstNotifDateLocal
    {
      get => FirstNotifDateOffset?.Date;
      set => FirstNotifDateOffset = value.HasValue
          ? new DateTimeOffset(value.Value.Date, TP.Now().Offset)
          : (DateTimeOffset?)null;
    }

    public string DeadlineTime { get; set; } = "18:00";
    public string FirstNotifTime { get; set; } = "09:00";

    public ObservableCollection<string> NotificationModes { get; } = new() { "toast", "nag" };
    public string NotificationMode { get; set; } = "nag";

    public ICommand SaveCommand { get; }

    public TaskEditorViewModel()
    {
      SaveCommand = new RelayCommand(w =>
      {
        if (string.IsNullOrWhiteSpace(Title))
        {
          System.Windows.MessageBox.Show("Title required");
          return;
        }
        if (w is Window win) win.DialogResult = true;
      });
    }

    public TaskItem ToTaskItem()
    {
      var now = TP.Now();

      var deadline = DeadlineDateOffset.HasValue
        ? DeadlineDateOffset.Value.CombineTime(DeadlineTime)
        : (DateTimeOffset?)null;

      var first = FirstNotifDateOffset.HasValue
        ? FirstNotifDateOffset.Value.CombineTime(FirstNotifTime)
        : now;

      return new TaskItem
      {
        Id = Guid.NewGuid().ToString(),
        Title = Title,
        Notes = Notes,
        CreatedAt = now,
        DeadlineAt = deadline,
        FirstNotificationAt = first,
        NextNotificationAt = first,
        NotificationMode = NotificationMode,
      };
    }

    public static TaskEditorViewModel FromTask(TaskItem t)
    {
      var vm = new TaskEditorViewModel
      {
        Title = t.Title,
        Notes = t.Notes,
        DeadlineDateOffset = t.DeadlineAt,
        FirstNotifDateOffset = t.FirstNotificationAt,
        DeadlineTime = t.DeadlineAt?.ToString("HH:mm") ?? "18:00",
        FirstNotifTime = t.FirstNotificationAt.ToString("HH:mm"),
        NotificationMode = t.NotificationMode,
      };
      return vm;
    }

    public void ApplyTo(TaskItem t)
    {
      t.Title = Title;
      t.Notes = Notes;

      t.DeadlineAt = DeadlineDateOffset.HasValue
        ? DeadlineDateOffset.Value.CombineTime(DeadlineTime)
        : null;

      if (FirstNotifDateOffset.HasValue)
      {
        var newFirst = FirstNotifDateOffset.Value.CombineTime(FirstNotifTime);
        t.FirstNotificationAt = newFirst;
        if (newFirst > TP.Now())
          t.NextNotificationAt = newFirst;
      }

      t.NotificationMode = NotificationMode;
    }
  }
}
