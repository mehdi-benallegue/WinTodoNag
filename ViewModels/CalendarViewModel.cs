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
    private DateTime _currentMonth = DateTime.Now.StartOfMonth();
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

      // Optional: rebuild when tasks change on disk
      StorageService.DataChanged += () => Build();

      Build();
    }

    private void Build()
    {
      MonthCells.Clear();
      var first = CurrentMonth.StartOfMonth();
      var start = first.AddDays(-(int)first.DayOfWeek);
      for (int i = 0; i < 42; i++)
      {
        var day = start.AddDays(i);
        var cell = new CalendarCell { Date = day };
        foreach (var t in StorageService.Current.Tasks)
        {
          if (t.DeadlineAt?.Date == day.Date) cell.Entries.Add($"Deadline: {t.Title}");
          if (t.NextNotificationAt.Date == day.Date) cell.Entries.Add($"Notify: {t.Title} @ {t.NextNotificationAt:HH:mm}");
        }
        MonthCells.Add(cell);
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }
}
