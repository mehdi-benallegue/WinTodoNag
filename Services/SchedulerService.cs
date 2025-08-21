using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using WinTodoNag.Models;


namespace WinTodoNag.Services
{
  public static class SchedulerService
  {
    private static readonly Timer _timer = new(15000); // 15s tick
    private static List<TaskItem> _all = new();


    public static void Initialize(IEnumerable<TaskItem> tasks)
    {
      _all = Flatten(tasks).ToList();
      _timer.AutoReset = true;
      _timer.Elapsed += (_, __) => Tick();
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


    private static void Tick()
    {
      var now = DateTime.Now;
      foreach (var t in _all.Where(t => t.CompletedAt == null && t.NextNotificationAt <= now).OrderBy(t => t.NextNotificationAt))
      {
        Notify(t);
      }
    }


    private static void Notify(TaskItem t)
    {
      if (t.NotificationMode == "toast") NotificationService.ShowToast(t);
      else NagService.ShowNag(t);


      // If ignored, re-fire 1 minute later. For toast/nag we set next to now+1m and the dialog/service will update again if user acts.
      t.NextNotificationAt = DateTime.Now.AddMinutes(1);
      StorageService.Save();
    }
  }
}