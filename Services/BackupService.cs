using System.IO;

namespace WinTodoNag.Services
{
  public static class BackupService
  {
    /// <summary>
    /// Keeps N rolling backups alongside the main file.
    /// </summary>
    public static void RotateBackups(string path, int count = 10)
    {
      if (!File.Exists(path)) return;
      for (int i = count; i >= 1; i--)
      {
        var src = i == 1 ? path : path + $".bak{i - 1}";
        var dst = path + $".bak{i}";
        if (File.Exists(src)) File.Copy(src, dst, true);
      }
    }
  }
}
