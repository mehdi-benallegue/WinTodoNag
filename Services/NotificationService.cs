using WinTodoNag.Models;


namespace WinTodoNag.Services
{
  public static class NotificationService
  {
    public static void ShowToast(TaskItem t)
    {
      // Placeholder. Real implementation with CommunityToolkit toast and protocol activation or COM.
      System.Windows.Forms.NotifyIcon ni = TrayService.EnsureNotifyIcon();
      ni.BalloonTipTitle = "Task Reminder";
      ni.BalloonTipText = t.Title;
      ni.ShowBalloonTip(3000);
    }
  }
}