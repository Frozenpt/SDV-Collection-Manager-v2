using System;
using SDV_Collection_Manager_v2.Commands;
using SDV_Collection_Manager_v2;
using System.Xml.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Managers
{
    public class OptionsMenu
    {
        static XDocument doc = new XDocument();
        static string docPath = @"assets/Config.xml";
        // General Options
        public static string HelpNotes
        {
            get { return "1. Check the Options Menu (First Tab) for possible quality-of-life improvements." + Environment.NewLine + 
                    "2. Use the Middle Mouse Button on an item or event's image to open its Wiki page." + Environment.NewLine + 
                    "3. Mousewheel allows you to scroll through pages and seasons."; }
        }
        // Calendar Options
        #region Greenhouse Mode
        private static bool _GreenHouseMode;
        public static bool GreenHouseMode
        {
            get { return _GreenHouseMode; }
            set { _GreenHouseMode = value; updateCropLists(); }
        }
        public static void updateCropLists()
        {
            if (!GreenHouseMode)
            {
                foreach (string season in CalendarManager.seasons)
                {
                    switch (season)
                    {
                        case "spring":
                            setFilter(CalendarManager.Spring);
                            CalendarManager.removeNonSeasonCrops(CalendarManager.Spring);
                            break;
                        case "summer":
                            setFilter(CalendarManager.Summer);
                            CalendarManager.removeNonSeasonCrops(CalendarManager.Summer);
                            break;
                        case "fall":
                            setFilter(CalendarManager.Fall);
                            CalendarManager.removeNonSeasonCrops(CalendarManager.Fall);
                            break;
                        case "winter":
                            setFilter(CalendarManager.Winter);
                            CalendarManager.removeNonSeasonCrops(CalendarManager.Winter);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                foreach (string season in CalendarManager.seasons)
                {
                    switch (season)
                    {
                        case "spring":
                            resetFilter(CalendarManager.Spring);
                            break;
                        case "summer":
                            resetFilter(CalendarManager.Summer);
                            break;
                        case "fall":
                            resetFilter(CalendarManager.Fall);
                            break;
                        case "winter":
                            resetFilter(CalendarManager.Winter);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private static void setFilter(ObservableCollection<Day> season)
        {
            CalendarManager.seasonToFilter = season[0].Season;
            foreach (Day day in season)
            {
                day.CropListView.Filter = CalendarManager.addCropListFilter;
                day.CropListView.Refresh();
            }
        }
        private static void resetFilter(ObservableCollection<Day> season)
        {
            foreach (Day day in season)
            {
                day.CropListView.Filter = CalendarManager.removeCropListFilter;
                day.CropListView.Refresh();
            }
        }
        private static bool _IconOnYield;
        public static bool IconOnYield
        {
            get { return _IconOnYield; }
            set { _IconOnYield = value; swapIcons(); }
        }
        public static void swapIcons()
        {
            foreach (string season in CalendarManager.seasons)
            {
                switch (season)
                {
                    case "spring":
                        CalendarManager.swapPlantedCropsDay(CalendarManager.Spring);
                        break;
                    case "summer":
                        CalendarManager.swapPlantedCropsDay(CalendarManager.Summer);
                        break;
                    case "fall":
                        CalendarManager.swapPlantedCropsDay(CalendarManager.Fall);
                        break;
                    case "winter":
                        CalendarManager.swapPlantedCropsDay(CalendarManager.Winter);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        // Farm Planner Options
        #region Selected Farm Layout
        private static Layout _selectedLayout;
        public static Layout SelectedLayout
        {
            get
            {
                if (_selectedLayout == null)
                {
                    _selectedLayout = new Layout();
                }
                return _selectedLayout;
            }
            set
            {
                _selectedLayout = value;
                AdjustSize();
            }
        }
        #endregion
        #region Layout Resize
        static bool isRuntime = true;
        static int originalWidth;
        static int originalHeight;
        private static bool IsFarmPlannerOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Visibility == Visibility.Visible)
                {
                    return true;
                }
            }
            return false;
        }
        private static int _LayoutSize;
        public static int LayoutSize
        {
            get { return _LayoutSize; }
            set { _LayoutSize = value; OnStaticPropertyChanged("LayoutSize"); AdjustSize(); }
        }
        private static void AdjustSize()
        {
            if (isRuntime)
            {
                decimal percentagetoreduce = decimal.Divide(LayoutSize, 100);
                LayoutWidth = Convert.ToInt32(originalWidth * percentagetoreduce);
                LayoutHeight = LayoutWidth * originalHeight / originalWidth;
                isRuntime = false;
                return;
            }
            if (IsFarmPlannerOpen())
            {
                if (MessageBox.Show("Resizing the planner may freeze the Stardew Valley Manager for a few seconds.\nDo you wish to continue?",
                     "Heavy loading", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    decimal percentagetoreduce = decimal.Divide(LayoutSize, 100);
                    LayoutWidth = Convert.ToInt32(originalWidth * percentagetoreduce);
                    LayoutHeight = LayoutWidth * originalHeight / originalWidth;
                    
                }
            }
            
        }
        private static int _LayoutWidth;
        public static int LayoutWidth
        {
            get { return _LayoutWidth; }
            set
            {
                if (value != _LayoutWidth)
                {
                    _LayoutWidth = value;
                    OnStaticPropertyChanged("LayoutWidth");
                }
            }
        }
        private static int _LayoutHeight;
        public static int LayoutHeight
        {
            get { return _LayoutHeight; }
            set {
                if (value != _LayoutWidth)
                {
                    _LayoutHeight = value;
                    OnStaticPropertyChanged("LayoutHeight");
                }
            }
        }
        #endregion
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        private static void OnStaticPropertyChanged(string propertyName)
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
            }
        }
        private static XElement getElement(string name)
        {
            return doc.Root.Element(name);
        }
        public static void loadConfig()
        {
            doc = XDocument.Load(docPath);
            GreenHouseMode = bool.Parse(getElement("greenhousemode").Value);
            originalWidth = int.Parse(getElement("layoutwidth").Value);
            originalHeight = int.Parse(getElement("layoutheight").Value);
            LayoutWidth = int.Parse(getElement("layoutwidth").Value);
            LayoutHeight = int.Parse(getElement("layoutheight").Value);
            LayoutSize = int.Parse(getElement("layoutsize").Value);
            SelectedLayout = !string.IsNullOrWhiteSpace(getElement("selectedlayout").Value) ?
                FarmManager.getLayout(getElement("selectedlayout").Value) : FarmManager.getLayout("Standard");
        }
        public static void saveConfig()
        {
            getElement("greenhousemode").Value = GreenHouseMode.ToString();
            getElement("selectedlayout").Value = SelectedLayout.Name;
            getElement("layoutsize").Value = LayoutSize.ToString();
            doc.Save(docPath);
        }
    }
}
