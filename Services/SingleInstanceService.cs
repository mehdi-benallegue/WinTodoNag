using System.Threading;


namespace WinTodoNag.Services
{
  public static class SingleInstanceService
  {
    private static Mutex? _mutex;
    public static bool TryClaim(string name)
    {
      bool created;
      _mutex = new Mutex(true, $"Global/{name}", out created);
      return created;
    }
  }
}