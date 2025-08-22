using System;
using System.Collections.ObjectModel;

namespace WinTodoNag.Models
{
  public class TaskItem
  {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = Services.TimeProvider.Now();
    public DateTimeOffset? DeadlineAt { get; set; }
    public DateTimeOffset FirstNotificationAt { get; set; } = Services.TimeProvider.Now();
    public DateTimeOffset NextNotificationAt { get; set; } = Services.TimeProvider.Now();

    // "toast" | "nag"
    public string NotificationMode { get; set; } = "nag";

    public DateTimeOffset? CompletedAt { get; set; }
    // "manual" | "recursive" | null
    public string? CompletedOrigin { get; set; }

    public ObservableCollection<TaskItem> Subtasks { get; set; } = new();

    public bool IsCompleted
    {
      get => CompletedAt != null;
      set
      {
        if (value && CompletedAt == null) MarkCompletedRecursive(manual: true);
        else if (!value && CompletedAt != null) UncheckWithRules();
      }
    }

    public void MarkCompletedRecursive(bool manual)
    {
      var now = Services.TimeProvider.Now();
      CompletedAt = now;
      CompletedOrigin = manual ? "manual" : "recursive";
      foreach (var c in Subtasks)
      {
        if (!c.IsCompleted) c.MarkCompletedRecursive(manual: false);
      }
    }

    public void UncheckWithRules()
    {
      CompletedAt = null;
      CompletedOrigin = null;
      foreach (var c in Subtasks)
      {
        if (c.CompletedAt != null && c.CompletedOrigin == "recursive")
        {
          c.CompletedAt = null; c.CompletedOrigin = null; c.UncheckChildrenIfRecursive();
        }
      }
    }

    private void UncheckChildrenIfRecursive()
    {
      foreach (var c in Subtasks)
      {
        if (c.CompletedAt != null && c.CompletedOrigin == "recursive")
        {
          c.CompletedAt = null; c.CompletedOrigin = null; c.UncheckChildrenIfRecursive();
        }
      }
    }
  }
}
