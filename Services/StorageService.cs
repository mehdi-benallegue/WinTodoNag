using System;                 // <-- make sure this is present
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

  public static class StorageService
  {
    // NEW: notify listeners (e.g., Calendar) when data changes
    public static event Action? DataChanged;

    public static RootDoc Current { get; private set; } = new();
    private static readonly ISerializer _ser =
        new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
    private static readonly IDeserializer _des =
        new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

    public static string FilePath { get; private set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WinTodoNag", "todo.yaml");

    public static void SetFilePath(string path)
    {
      FilePath = path;
      Save();
    }

    public static void LoadOrInit()
    {
      Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
      if (File.Exists(FilePath))
      {
        var txt = File.ReadAllText(FilePath);
        Current = _des.Deserialize<RootDoc>(txt) ?? new();
      }
      else
      {
        Current = new();
        Save();
      }
      DataChanged?.Invoke();   // << fire after loading
    }

    public static void Save()
    {
      BackupService.RotateBackups(FilePath);
      Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
      var yaml = _ser.Serialize(Current);
      File.WriteAllText(FilePath, yaml);
      DataChanged?.Invoke();   // << fire after saving
    }

    public static void RevealDataFile()
    {
      var folder = Path.GetDirectoryName(FilePath)!;
      if (Directory.Exists(folder))
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = folder, UseShellExecute = true });
    }
  }
}
