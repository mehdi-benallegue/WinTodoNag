using System.Windows;


namespace WinTodoNag.Views
{
  public partial class NagDialog : Window
  {
    public NagDialog() { InitializeComponent(); }


    protected override void OnSourceInitialized(System.EventArgs e)
    {
      base.OnSourceInitialized(e);
      // Important: DO NOT steal focus. We keep Topmost true but don't call Activate() here.
    }
  }
}