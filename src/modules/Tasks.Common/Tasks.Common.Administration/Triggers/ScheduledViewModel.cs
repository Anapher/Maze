using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Prism.Mvvm;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Tasks.Infrastructure.Core;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Triggers
{
    public class ScheduledViewModel : BindableBase, ITriggerViewModel<ScheduledTriggerInfo>
    {
        private ScheduleMode _mode;
        private bool _monthlyAtRelativeDays;
        private int _recurEvery;
        private DateTime _startTime;
        private bool _synchronizeTimeZone;

        public ScheduledViewModel()
        {
            var currentCulture = CultureInfo.GetCultureInfo(Tx.CurrentThreadCulture);
            var days = new[]
            {
                DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };

            StartTime = DateTime.Now;

            WeekDays = new List<ListViewModel<DayOfWeek>>();
            for (var i = 0; i < 7; i++)
                WeekDays.Add(new ListViewModel<DayOfWeek>(days[i], currentCulture.DateTimeFormat.DayNames[i]));

            MonthDays = Enumerable.Range(1, 31).Select(x => new ListViewModel<int>(x, x.ToString())).ToList();
            MonthDays.Add(new ListViewModel<int>(-1, Tx.T("TasksCommon:Triggers.Scheduled.View.Last")));

            Months = Enumerable.Range(0, 11).Select(x => new ListViewModel<int>(x, currentCulture.DateTimeFormat.MonthNames[x])).ToList();
            RelativeDays = Enum.GetValues(typeof(RelativeDayInMonth)).Cast<RelativeDayInMonth>().Select(x =>
                new ListViewModel<RelativeDayInMonth>(x, Tx.T($"TasksCommon:Triggers.Scheduled.View.DayInMonth.{x}"))).ToList();
        }

        public List<ListViewModel<DayOfWeek>> WeekDays { get; }
        public List<ListViewModel<int>> MonthDays { get; }
        public List<ListViewModel<int>> Months { get; }
        public List<ListViewModel<RelativeDayInMonth>> RelativeDays { get; }

        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        public ScheduleMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        public bool MonthlyAtRelativeDays
        {
            get => _monthlyAtRelativeDays;
            set => SetProperty(ref _monthlyAtRelativeDays, value);
        }

        public bool SynchronizeTimeZone
        {
            get => _synchronizeTimeZone;
            set => SetProperty(ref _synchronizeTimeZone, value);
        }

        public int RecurEvery
        {
            get => _recurEvery;
            set => SetProperty(ref _recurEvery, value);
        }

        public void Initialize(ScheduledTriggerInfo model)
        {
        }

        public ValidationResult ValidateInput() => ValidationResult.Success;
        public ValidationResult ValidateContext(MazeTask mazeTask) => ValidationResult.Success;
        public ScheduledTriggerInfo Build() => throw new NotImplementedException();
    }

    public class ListViewModel<TValue> : BindableBase
    {
        private bool _isSelected;

        public ListViewModel(TValue value, string name)
        {
            Value = value;
            Name = name;
        }

        public TValue Value { get; }
        public string Name { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}