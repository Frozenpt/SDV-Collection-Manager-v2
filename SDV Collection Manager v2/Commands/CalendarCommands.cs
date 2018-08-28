using System;
using System.Windows.Input;
using Managers;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Commands
{
    public class SelectedDay : ICommand
    {
        public CalendarManager Calendar { get; set; }

        public SelectedDay(CalendarManager calendar)
        {
            this.Calendar = calendar;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Day)
                { return true; }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            CalendarManager.currentSelectedDay = parameter as Day;
        }
    }
    public class IsCropChecked : ICommand
    {
        public CalendarManager Calendar { get; set; }

        public IsCropChecked(CalendarManager calendar)
        {
            this.Calendar = calendar;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Crop)
                { return true; }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            Crop crop = parameter as Crop;
            if (crop.IsChecked)
            {
                CalendarManager.updateNote(crop, "remove", false);
            }
            else
            {
                CalendarManager.updateNote(crop, "add", false);
            }
        }
    }
    public class OpenWebPage : ICommand
    {
        public CalendarManager Calendar { get; set; }

        public OpenWebPage(CalendarManager calendar)
        {
            this.Calendar = calendar;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Day)
                {
                    return true;
                }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            Day day = parameter as Day;
            if (!string.IsNullOrWhiteSpace(day.EventName))
            {
                System.Diagnostics.Process.Start("http://stardewvalleywiki.com/" + day.EventName);
            }
        }
    }
    public class ResetSeason : ICommand
    {
        public CalendarManager Calendar { get; set; }

        public ResetSeason(CalendarManager calendar)
        {
            this.Calendar = calendar;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is string)
                { return true; }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            string season = parameter as string;
            switch (season)
            {
                case "Spring":
                    CalendarManager.resetSeasonNotes(CalendarManager.Spring);
                    break;
                case "Summer":
                    CalendarManager.resetSeasonNotes(CalendarManager.Summer);
                    break;
                case "Fall":
                    CalendarManager.resetSeasonNotes(CalendarManager.Fall);
                    break;
                case "Winter":
                    CalendarManager.resetSeasonNotes(CalendarManager.Winter);
                    break;
                default:
                    break;
            }
            
        }
    }
}
