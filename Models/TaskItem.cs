using System;
using System.Collections.ObjectModel;
using System.Linq;


namespace WinTodoNag.Models
{
  public class TaskItem
  {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? DeadlineAt { get; set; }
    public DateTime FirstNotificationAt { get; set; } = DateTime.Now;
    public DateTime NextNotificationAt { get; set; } = DateTime.Now;
    public string NotificationMode { get; set; } = "nag"; // "toast" | "nag"
    public DateTime? CompletedAt { get; set; }
    public string? CompletedOrigin { get; set; } // "manual" | "recursive" | null


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
      var now = DateTime.Now;
      CompletedAt = now;
      CompletedOrigin = manual ? "manual" : "recursive";
      foreach (var c in Subtasks)
      {
        if (!c.IsCompleted) c.MarkCompletedRecursive(manual: false);
      }
    }


    public void UncheckWithRules()
    {
      // Uncheck parent
      CompletedAt = null; // origin stays as record of last state is not needed when unchecked
                          // Uncheck only children completed recursively
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