using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WinTodoNag.Models;
using WinTodoNag.Utils;


namespace WinTodoNag.ViewModels
{
  public class TaskEditorViewModel
  {
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime? DeadlineDate { get; set; }
    public string DeadlineTime { get; set; } = "18:00";
    public DateTime? FirstNotifDate { get; set; } = DateTime.Now.Date;
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
      var now = DateTime.Now;
      return new TaskItem
      {
        Id = Guid.NewGuid().ToString(),
        Title = Title,
        Notes = Notes,
        CreatedAt = now,
        DeadlineAt = DeadlineDate.HasValue ? DeadlineDate.Value.CombineTime(DeadlineTime) : null,
        FirstNotificationAt = FirstNotifDate.HasValue ? FirstNotifDate.Value.CombineTime(FirstNotifTime) : now,
        NextNotificationAt = FirstNotifDate.HasValue ? FirstNotifDate.Value.CombineTime(FirstNotifTime) : now,
        NotificationMode = NotificationMode,
      };
    }


    public static TaskEditorViewModel FromTask(TaskItem t)
    {
      var vm = new TaskEditorViewModel
      {
        Title = t.Title,
        Notes = t.Notes,
        DeadlineDate = t.DeadlineAt?.Date,
        DeadlineTime = t.DeadlineAt?.ToString("HH:mm") ?? "18:00",
        FirstNotifDate = t.FirstNotificationAt.Date,
        FirstNotifTime = t.FirstNotificationAt.ToString("HH:mm"),
        NotificationMode = t.NotificationMode,
      };
      return vm;
    }


    public void ApplyTo(TaskItem t)
    {
      t.Title = Title; t.Notes = Notes;
      t.DeadlineAt = DeadlineDate.HasValue ? DeadlineDate.Value.CombineTime(DeadlineTime) : null;
      t.FirstNotificationAt = FirstNotifDate.HasValue ? FirstNotifDate.Value.CombineTime(FirstNotifTime) : t.FirstNotificationAt;
      t.NotificationMode = NotificationMode;
    }
  }
}