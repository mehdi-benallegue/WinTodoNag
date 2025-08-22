using System;
using System.Collections.Generic;
using System.Linq;
using WinTodoNag.Models;
using TP = WinTodoNag.Services.TimeProvider;

namespace WinTodoNag.Services
{
  public static class SchedulerService
  {
    // Force System.Timers.Timer explicitly
    private static readonly System.Timers.Timer _timer = new System.Timers.Timer(15000);
    private static List<TaskItem> _all = new();

    public static void Initialize(IEnumerable<TaskItem> tasks)
    {
      _all = Flatten(tasks).ToList();
      _timer.AutoReset = true;
      _timer.Elapsed -= OnTick;
      _timer.Elapsed += OnTick;
      _timer.Start();
    }

    private static IEnumerable<TaskItem> Flatten(IEnumerable<TaskItem> roots)
    {
      foreach (var r in roots)
      {
        yield return r;
        foreach (var c in Flatten(r.Subtasks)) yield return c;
      }
    }

    // Fully qualify the event args type too
    private static void OnTick(object? _, System.Timers.ElapsedEventArgs __) => Tick();

    private static void Tick()
    {
      var now = TP.Now();
      foreach (var t in _all.Where(t => t.CompletedAt == null && t.NextNotificationAt <= now)
                            .OrderBy(t => t.NextNotificationAt))
      {
        Notify(t);
      }
    }

    private static void Notify(TaskItem t)
    {
      if (t.NotificationMode == "toast") NotificationService.ShowToast(t);
      else NagService.ShowNag(t);

      t.NextNotificationAt = TP.Now().AddMinutes(1);
      StorageService.Save();
    }

    public static void Reschedule(TaskItem t, DateTimeOffset newDue)
    {
      t.NextNotificationAt = newDue;
      StorageService.Save();
    }
  }
}
