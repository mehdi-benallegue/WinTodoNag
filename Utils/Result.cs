namespace WinTodoNag.Utils
{
  public readonly struct Result
  {
    public bool Ok { get; }
    public string? Error { get; }
    private Result(bool ok, string? err) { Ok = ok; Error = err; }
    public static Result Success() => new(true, null);
    public static Result Fail(string e) => new(false, e);
  }
}