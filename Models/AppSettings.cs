namespace WinTodoNag.Models
{
  public class AppSettings
  {
    public string DataFilePath { get; set; } = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "WinTodoNag", "todo.yaml");
    public bool RunAtStartup { get; set; } = true;
    public bool MinimizeToTray { get; set; } = true;
    public bool AllowQuitFromTray { get; set; } = false;
  }
}