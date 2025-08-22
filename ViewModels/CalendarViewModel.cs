using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WinTodoNag.Services;
using WinTodoNag.Utils;

namespace WinTodoNag.ViewModels
{
  public class CalendarCell
  {
    public DateTime Date { get; set; }
    public ObservableCollection<string> Entries { get; set; } = new();
  }

  public class CalendarViewModel : INotifyPropertyChanged
  {
    private DateTime _currentMonth = DateTimeExtensions.StartOfMonth(DateTime.Now);
    public DateTime CurrentMonth
    {
      get => _currentMonth;
      set
      {
        if (_currentMonth != value)
        {
          _currentMonth = value;
          OnPropertyChanged();
          Build();
        }
      }
    }

    public ObservableCollection<CalendarCell> MonthCells { get; } = new();

    public ICommand PrevMonthCommand { get; }
    public ICommand NextMonthCommand { get; }

    public CalendarViewModel()
    {
      PrevMonthCommand = new RelayCommand(_ => CurrentMonth = CurrentMonth.AddMonths(-1));
      NextMonthCommand = new RelayCommand(_ => CurrentMonth = CurrentMonth.AddMonths(1));

      StorageService.DataChanged += () => Build();
      Build();
    }

    private static System.Collections.Generic.IEnumerable<Models.TaskItem> Flatten(System.Collections.Generic.IEnumerable<Models.TaskItem> roots)
    {
      foreach (var r in roots)
      {
        yield return r;
        foreach (var c in Flatten(r.Subtasks)) yield return c;
      }
    }

    private void Build()
    {
      MonthCells.Clear();
      var first = DateTimeExtensions.StartOfMonth(CurrentMonth);
      var start = first.AddDays(-(int)first.DayOfWeek);

      for (int i = 0; i < 42; i++)
      {
        var day = start.AddDays(i);
        var cell = new CalendarCell { Date = day };

        foreach (var t in Flatten(StorageService.Current.Tasks))
        {
          if (t.DeadlineAt?.LocalDateTime.Date == day.Date)
            cell.Entries.Add($"Deadline: {t.Title}");

          if (t.NextNotificationAt.LocalDateTime.Date == day.Date)
            cell.Entries.Add($"Notify: {t.Title} @ {t.NextNotificationAt:HH:mm}");
        }

        MonthCells.Add(cell);
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }
}
