using System;
using System.Collections.Generic;
using System.Linq;
using WinTodoNag.Models;
using TP = WinTodoNag.Services.TimeProvider;

namespace WinTodoNag.Services
{
  /// <summary>
  /// Simple scheduler that:
  /// - Watches storage changes so new/edited tasks are considered immediately.
  /// - Fires due tasks in order.
  /// - Auto-refires in 1 minute if ignored, but stops as soon as the task is completed or snoozed.
  /// </summary>
  public static class SchedulerService
  {
    private static readonly System.Timers.Timer _timer = new(15000); // 15s idle tick
    private static readonly object _lock = new();
    private static List<TaskItem> _all = new();

    public static void Initialize(IEnumerable<TaskItem> initialTasks)
    {
      RefreshTasks(initialTasks);

      // Rebuild flat list on any load/save
      StorageService.DataChanged -= OnDataChanged;
      StorageService.DataChanged += OnDataChanged;

      _timer.AutoReset = true;
      _timer.Elapsed -= OnTick;
      _timer.Elapsed += OnTick;
      _timer.Start();
    }

    private static void OnDataChanged()
    {
      RefreshTasks(StorageService.Current.Tasks);
      // optional: run a quick tick to pick up newly-due items
      Tick();
    }

    private static void RefreshTasks(IEnumerable<TaskItem> roots)
    {
      lock (_lock)
      {
        _all = Flatten(roots).ToList();
      }
    }

    private static IEnumerable<TaskItem> Flatten(IEnumerable<TaskItem> roots)
    {
      foreach (var r in roots)
      {
        yield return r;
        foreach (var c in Flatten(r.Subtasks)) yield return c;
      }
    }

    private static void OnTick(object? _, System.Timers.ElapsedEventArgs __) => Tick();

    private static void Tick()
    {
      List<TaskItem> snapshot;
      lock (_lock) snapshot = _all;

      var now = TP.Now();
      foreach (var t in snapshot
               .Where(t => t.CompletedAt == null && t.NextNotificationAt <= now)
               .OrderBy(t => t.NextNotificationAt)
               .ThenBy(t => t.DeadlineAt ?? DateTimeOffset.MaxValue)
               .ThenBy(t => t.CreatedAt))
      {
        Notify(t);
      }
    }

    private static void Notify(TaskItem t)
    {
      if (t.NotificationMode == "toast")
        NotificationService.ShowToast(t);
      else
        NagService.ShowNag(t);

      // Only push the 1-min refire if still incomplete AND the task wasn't snoozed during this call.
      // We test by checking if its NextNotificationAt is still <= now.
      var now = TP.Now();
      if (t.CompletedAt == null && t.NextNotificationAt <= now)
      {
        t.NextNotificationAt = now.AddMinutes(1);
        StorageService.Save();
      }
    }

    public static void Reschedule(TaskItem t, DateTimeOffset newDue)
    {
      t.NextNotificationAt = newDue;
      StorageService.Save(); // triggers DataChanged; list & calendar update
    }
  }
}
