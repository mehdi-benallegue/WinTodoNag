using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using WinTodoNag.Models;

namespace WinTodoNag.Services
{
  public class RootDoc
  {
    public int Version { get; set; } = 1;
    public AppSettings Settings { get; set; } = new();
    public List<TaskItem> Tasks { get; set; } = new();
  }

  /// <summary>
  /// YAML load/save with:
  /// - Atomic save (tmp + Replace when dest exists, else Move)
  /// - 10 rolling backups
  /// - External edit detection with reload prompt
  /// </summary>
  public static class StorageService
  {
    public static event Action? DataChanged;

    public static RootDoc Current { get; private set; } = new();

    private static readonly ISerializer _ser =
      new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

    private static readonly IDeserializer _des =
      new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

    public static string FilePath { get; private set; } =
      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WinTodoNag", "todo.yaml");

    private static FileSystemWatcher? _fsw;
    private static string _lastSavedText = string.Empty;

    public static void SetFilePath(string path)
    {
      FilePath = path;
      LoadOrInit(); // rewire watcher and save if needed
    }

    public static void LoadOrInit()
    {
      Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
      if (File.Exists(FilePath))
      {
        var txt = File.ReadAllText(FilePath);
        _lastSavedText = txt;
        Current = _des.Deserialize<RootDoc>(txt) ?? new();
      }
      else
      {
        Current = new();
        Save(); // creates the file
      }

      WatchFile();
      DataChanged?.Invoke();
    }

    public static void Save()
    {
      // Rotate only if dest exists already
      if (File.Exists(FilePath))
        BackupService.RotateBackups(FilePath, 10);

      Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

      var yaml = _ser.Serialize(Current);
      var tmp = FilePath + ".tmp";
      File.WriteAllText(tmp, yaml);

      // If destination exists, Replace (atomic); otherwise first save uses Move
      if (File.Exists(FilePath))
      {
        File.Replace(tmp, FilePath, null);
      }
      else
      {
        File.Move(tmp, FilePath);
      }

      _lastSavedText = yaml;
      DataChanged?.Invoke();
    }

    public static void RevealDataFile()
    {
      var folder = Path.GetDirectoryName(FilePath)!;
      if (Directory.Exists(folder))
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = folder, UseShellExecute = true });
    }

    private static void WatchFile()
    {
      _fsw?.Dispose();
      var dir = Path.GetDirectoryName(FilePath)!;
      var name = Path.GetFileName(FilePath);
      _fsw = new FileSystemWatcher(dir, name)
      {
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
      };
      _fsw.Changed += (_, __) => OnExternalChanged();
      _fsw.EnableRaisingEvents = true;
    }

    private static void OnExternalChanged()
    {
      try
      {
        var txt = File.ReadAllText(FilePath);
        if (txt == _lastSavedText) return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
          var r = System.Windows.MessageBox.Show(
            "The data file was changed externally.\nReload it now? Unsaved changes in the app will be lost.",
            "File changed",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

          if (r == System.Windows.MessageBoxResult.Yes)
          {
            Current = _des.Deserialize<RootDoc>(txt) ?? new();
            _lastSavedText = txt;
            DataChanged?.Invoke();
          }
        });
      }
      catch
      {
        // OneDrive/AV/other processes may lock briefly; ignore and let the next event handle it.
      }
    }
  }
}
