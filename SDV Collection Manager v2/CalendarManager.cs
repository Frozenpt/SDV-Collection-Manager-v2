using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Commands;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Generic;

namespace Managers
{
    public class CalendarManager
    {
        private static ObservableCollection<Day> _Spring;
        public static ObservableCollection<Day> Spring
        {
            get
            {
                if (_Spring == null)
                {
                    _Spring = new ObservableCollection<Day>();
                }
                return _Spring;
            }
            set
            {
                _Spring = value;
            }
        }
        private static ObservableCollection<Day> _Summer;
        public static ObservableCollection<Day> Summer
        {
            get
            {
                if (_Summer == null)
                {
                    _Summer = new ObservableCollection<Day>();
                }
                return _Summer;
            }
            set
            {
                _Summer = value;
            }
        }
        private static ObservableCollection<Day> _Fall;
        public static ObservableCollection<Day> Fall
        {
            get
            {
                if (_Fall == null)
                {
                    _Fall = new ObservableCollection<Day>();
                }
                return _Fall;
            }
            set
            {
                _Fall = value;
            }
        }
        private static ObservableCollection<Day> _Winter;
        public static ObservableCollection<Day> Winter
        {
            get
            {
                if (_Winter == null)
                {
                    _Winter = new ObservableCollection<Day>();
                }
                return _Winter;
            }
            set
            {
                _Winter = value;
            }
        }
        public static string[] seasons = new string[] { "spring", "summer", "fall", "winter" };
        public static string seasonToFilter = "";
        static XDocument doc = new XDocument();
        static string docPath = @"assets/Calendar.xml";
        private static bool wasRemoved { get; set; }
        public static Day currentSelectedDay;
        public static SelectedDay _selectedDay { get; set; }
        public static IsCropChecked _isCropChecked { get; set; }
        public static OpenWebPage _openWebPage { get; set; }
        public static ResetSeason _resetSeason { get; set; }
        public CalendarManager()
        {
            _selectedDay = new SelectedDay(this);
            _isCropChecked = new IsCropChecked(this);
            _openWebPage = new OpenWebPage(this);
            _resetSeason = new ResetSeason(this);
        }
        #region Build Calendar Days
        private static ObservableCollection<Crop> buildCropList(string seasonName, string note)
        {
            ObservableCollection<Crop> _cropList = new ObservableCollection<Crop>();
            int cont = 0;
            foreach (XElement e in doc.Descendants("crops").Descendants("crop"))
            {
                _cropList.Add(new Crop
                {
                    Name = e.Attribute("name").Value,
                    Days = int.Parse(e.Attribute("days").Value),
                    Cont = int.TryParse(e.Attribute("cont").Value, out cont) ? int.Parse(e.Attribute("cont").Value) : 0,
                    Season = e.Attribute("season").Value,
                    Image = "images/shippeditems/" + e.Attribute("name").Value + ".png",
                    IsChecked = note.Contains(e.Attribute("name").Value + " planted.") ? true : false,
                    IconOnYield = bool.Parse(e.Attribute("icononyield").Value),
                });
            }
            return _cropList;
        }
        private static void buildPlantedList(ObservableCollection<Day> season)
        {
            foreach (Day day in season)
            {
                foreach (Crop crop in day.CropList)
                {
                    if (crop.IsChecked)
                    {
                        day.PlantedCrops.Add(crop);
                    }
                }
            }
        }
        private static Day newDay(XElement e, string seasonName)
        {
            ObservableCollection<Crop> plantedCrops = new ObservableCollection<Crop>();
            Day day = new Day
            {
                Season = seasonName,
                ID = int.Parse(e.Attribute("id").Value),
                Event = e.Attribute("event") != null ? "images/calendar/" + e.Attribute("event").Value + ".png" : "images/calendar/Filler.png",
                EventName = e.Attribute("event") != null ? e.Attribute("event").Value : "",
                PlayerNote = e.Attribute("playernote").Value,
                CropNote = e.Attribute("cropnote").Value,
                CropList = buildCropList(seasonName, e.Attribute("cropnote").Value),
            };
            return day;
        }
        public static void buildCalendar()
        {
            doc = XDocument.Load(docPath);
            foreach (string season in seasons)
            {
                foreach (XElement e in doc.Descendants(season).Descendants("day"))
                {
                    switch (season)
                    {
                        case "spring":
                            Spring.Add(newDay(e, season));
                            break;
                        case "summer":
                            Summer.Add(newDay(e, season));
                            break;
                        case "fall":
                            Fall.Add(newDay(e, season));
                            break;
                        case "winter":
                            Winter.Add(newDay(e, season));
                            break;
                        default:
                            break;
                    }
                }
                switch (season)
                {
                    case "spring":
                        buildPlantedList(Spring);
                        break;
                    case "summer":
                        buildPlantedList(Summer);
                        break;
                    case "fall":
                        buildPlantedList(Fall);
                        break;
                    case "winter":
                        buildPlantedList(Winter);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        #region CropLists Filter
        public static bool addCropListFilter(object obj)
        {
            Crop crop = obj as Crop;
            if (crop.Season.Contains(seasonToFilter))
            {
                return true;
            }
            return false;
        }
        public static bool removeCropListFilter(object obj)
        {
            return true;
        }
        #endregion
        #region Update Notes
        // Adds or Removes the crop accordingly
        public static void updateNote(Crop crop, string action, bool nonSeasonReset)
        {
            Day yieldingDay = new Day();
            int baseYield = currentSelectedDay.ID + crop.Days; // # of the day when the crop can be harvested (Same Month)
            int baseYieldNextMonth = baseYield - 28; // # of the day when the crop can be harvested (Next Month)
            int baseYieldAncientFruit = (baseYield - 28) - 28; // # of the day when the crop can be harvested (Third Month)
            int lastPlantableDay = (28 - crop.Days) > 0 ? 28 - crop.Days : 28; // # of the last day the crop can be planted
            string plantedOn = ""; // Notes the day the crop was planted
            string yieldingOn = ""; // Notes the day the crop can be harvested
            string currentSeason = currentSelectedDay.Season;
            string nextSeason = calculateNextSeason();
            string cropName = crop.Name.Replace('_', ' ');

            #region Calculates Yielding Day
            if (OptionsMenu.GreenHouseMode || action == "remove")
            {
                if (baseYield < 28)
                {
                    if (nonSeasonReset && currentSeason == "fall")
                    {
                        wasRemoved = false;
                        return;
                    }
                    #region Yielding in the same month
                    switch (currentSelectedDay.Season)
                    {
                        case "spring":
                            yieldingDay = Spring[baseYield];
                            break;
                        case "summer":
                            yieldingDay = Summer[baseYield];
                            break;
                        case "fall":
                            yieldingDay = Fall[baseYield];
                            break;
                        case "winter":
                            yieldingDay = Winter[baseYield];
                            break;
                        default:
                            break;
                    }
                    plantedOn = cropName + " planted. Yielding on day " + (baseYield + 1) + " of this season.";
                    yieldingOn = cropName + " from day " + currentSelectedDay.ID + " of this season yielding today.";
                    #endregion
                }
                else if (baseYieldNextMonth < 28)
                {
                    #region Yielding in the next season
                    switch (nextSeason)
                    {
                        case "spring":
                            yieldingDay = Spring[baseYieldNextMonth];
                            break;
                        case "summer":
                            yieldingDay = Summer[baseYieldNextMonth];
                            break;
                        case "fall":
                            yieldingDay = Fall[baseYieldNextMonth];
                            break;
                        case "winter":
                            yieldingDay = Winter[baseYieldNextMonth];
                            break;
                        default:
                            break;
                    }
                    plantedOn = cropName + " planted. Yielding on day " + (baseYieldNextMonth + 1) + " of " + nextSeason + ".";
                    yieldingOn = cropName + " from day " + currentSelectedDay.ID + " of " + currentSelectedDay.Season + " yielding today.";
                    #endregion
                }
                else
                {
                    #region Yielding in 2 seasons
                    switch (calculateThirdSeason(nextSeason))
                    {
                        case "spring":
                            yieldingDay = Spring[baseYieldAncientFruit];
                            break;
                        case "summer":
                            yieldingDay = Summer[baseYieldAncientFruit];
                            break;
                        case "fall":
                            yieldingDay = Fall[baseYieldAncientFruit];
                            break;
                        case "winter":
                            yieldingDay = Winter[baseYieldAncientFruit];
                            break;
                        default:
                            break;
                    }

                    plantedOn = cropName + " planted. Yielding on day " + (baseYieldAncientFruit + 1) + " of " + calculateThirdSeason(nextSeason) + ".";
                    yieldingOn = cropName + " from day " + currentSelectedDay.ID + " of " + currentSelectedDay.Season + " yielding today.";
                    #endregion
                }
            }
            else
            {
                #region Filters Winter days from possible yieldingDay

                if (crop.Name.Contains("Ancient") && (nextSeason == "winter" || calculateThirdSeason(nextSeason) == "winter" && baseYieldNextMonth >= 28))
                {
                    MessageBox.Show("This crop won't yield before its season ends.\n" +
                        crop.Name + " takes " + crop.Days + " days to grow. This crop has to be planted before day " + lastPlantableDay + " of Summer."
                        , "Yielding out of season!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (baseYield >= 28 && nextSeason == "winter")
                {
                    MessageBox.Show("This crop won't yield before its season ends.\n" +
                        crop.Name + " takes " + crop.Days + " days to grow. This crop has to be planted before day " + lastPlantableDay + " of " + currentSeason + "."
                        , "Yielding out of season!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                #endregion
                if (baseYield < 28)
                {
                    #region Yielding in the same month
                    switch (currentSelectedDay.Season)
                    {
                        case "spring":
                            yieldingDay = Spring[baseYield];
                            break;
                        case "summer":
                            yieldingDay = Summer[baseYield];
                            break;
                        case "fall":
                            yieldingDay = Fall[baseYield];
                            break;
                        case "winter":
                            yieldingDay = Winter[baseYield];
                            break;
                        default:
                            break;
                    }
                    plantedOn = cropName + " planted. Yielding on day " + (baseYield + 1) + " of this season.";
                    yieldingOn = cropName + " from day " + currentSelectedDay.ID + " of this season yielding today.";
                    #endregion
                }
                else if (baseYieldNextMonth < 28)
                {
                    #region Yielding in the next season
                    switch (nextSeason)
                    {
                        case "spring":
                            yieldingDay = Spring[baseYieldNextMonth];
                            break;
                        case "summer":
                            yieldingDay = Summer[baseYieldNextMonth];
                            break;
                        case "fall":
                            yieldingDay = Fall[baseYieldNextMonth];
                            break;
                        case "winter":
                            yieldingDay = Winter[baseYieldNextMonth];
                            break;
                        default:
                            break;
                    }
                    plantedOn = cropName + " planted. Yielding on day " + (baseYieldNextMonth + 1) + " of " + nextSeason + ".";
                    yieldingOn = cropName + " from day " + currentSelectedDay.ID + " of " + currentSelectedDay.Season + " yielding today.";
                    #endregion
                }
                else
                {
                    #region Yielding in 2 seasons
                    switch (calculateThirdSeason(nextSeason))
                    {
                        case "spring":
                            yieldingDay = Spring[baseYieldAncientFruit];
                            break;
                        case "summer":
                            yieldingDay = Summer[baseYieldAncientFruit];
                            break;
                        case "fall":
                            yieldingDay = Fall[baseYieldAncientFruit];
                            break;
                        case "winter":
                            yieldingDay = Winter[baseYieldAncientFruit];
                            break;
                        default:
                            break;
                    }

                    plantedOn = cropName + " planted. Yielding on day " + (baseYieldAncientFruit + 1) + " of " + calculateThirdSeason(nextSeason) + ".";
                    yieldingOn = cropName + " from day " + currentSelectedDay.ID + " of " + currentSelectedDay.Season + " yielding today.";
                    #endregion
                }
            }
            #endregion
            #region Execute Action
            if (action == "add")
            {
                currentSelectedDay.CropNote += plantedOn+"\n";
                yieldingDay.CropNote += yieldingOn+"\n";
                crop.IsChecked = true;
                if (OptionsMenu.IconOnYield)
                {
                    crop.IconOnYield = true;
                    yieldingDay.PlantedCrops.Add(crop);
                    return;
                }
                currentSelectedDay.PlantedCrops.Add(crop);
            }
            else if (action == "remove")
            {
                currentSelectedDay.CropNote = Regex.Replace(currentSelectedDay.CropNote, plantedOn + "\n", "");
                yieldingDay.CropNote = Regex.Replace(yieldingDay.CropNote, yieldingOn + "\n", "");
                crop.IsChecked = false;
                if (nonSeasonReset) { wasRemoved = true; return; }
                if (crop.IconOnYield)
                {
                    yieldingDay.PlantedCrops.Remove(crop);
                    crop.IconOnYield = false;
                    return;
                }
                yieldingDay.PlantedCrops.Remove(crop);
                currentSelectedDay.PlantedCrops.Remove(crop);
            }
            else if (action == "swapIcon")
            {
                if (crop.IconOnYield)
                {
                    currentSelectedDay.PlantedCrops.Add(crop);
                    yieldingDay.PlantedCrops.Remove(crop);
                    crop.IconOnYield = false;
                }
                else
                {
                    currentSelectedDay.PlantedCrops.Remove(crop);
                    yieldingDay.PlantedCrops.Add(crop);
                    crop.IconOnYield = true;
                }
            } 
            #endregion
        }
        private static string calculateNextSeason()
        {
            string nextSeason = "";
            switch (currentSelectedDay.Season)
            {
                case "spring":
                    nextSeason = "summer";
                    break;
                case "summer":
                    nextSeason = "fall";
                    break;
                case "fall":
                    nextSeason = "winter";
                    break;
                case "winter":
                    nextSeason = "spring";
                    break;
                default:
                    break;
            }
            return nextSeason;
        }
        private static string calculateThirdSeason(string nextSeason)
        {
            string thirdSeason = "";
            switch (nextSeason)
            {
                case "spring":
                    thirdSeason = "summer";
                    break;
                case "summer":
                    thirdSeason = "fall";
                    break;
                case "fall":
                    thirdSeason = "winter";
                    break;
                case "winter":
                    thirdSeason = "spring";
                    break;
                default:
                    break;
            }
            return thirdSeason;
        }
        // Reset All Season Notes 
        public static void resetSeasonNotes(ObservableCollection<Day> season)
        {
            foreach (Day day in season)
            {
                foreach (Crop crop in day.CropList)
                {
                    currentSelectedDay = day;
                    updateNote(crop, "remove", false);
                }
            }
            //switch (season)
            //{
            //    case "Spring":
            //        foreach (Day day in Spring)
            //        { day.Note = ""; day.PlantedCrops.Clear(); resetCropCheck(day); }
            //        break;
            //    case "Summer":
            //        foreach (Day day in Summer)
            //        { day.Note = ""; day.PlantedCrops.Clear(); resetCropCheck(day); }
            //        break;
            //    case "Fall":
            //        foreach (Day day in Fall)
            //        { day.Note = ""; day.PlantedCrops.Clear(); resetCropCheck(day); }
            //        break;
            //    case "Winter":
            //        foreach (Day day in Winter)
            //        { day.Note = ""; day.PlantedCrops.Clear(); resetCropCheck(day); }
            //        break;
            //    default:
            //        break;
            //}
        }
        public static void removeNonSeasonCrops(ObservableCollection<Day> season)
        {
            ObservableCollection<Crop> toRemoveList = new ObservableCollection<Crop>();
            foreach (Day day in season)
            {
                foreach (Crop crop in day.PlantedCrops)
                {
                    if (!crop.Season.Contains(day.Season) || day.CropNote.Contains("winter"))
                    {
                        currentSelectedDay = day;
                        updateNote(crop, "remove", true);
                        if (wasRemoved)
                        {
                            toRemoveList.Add(crop);
                        }
                    }
                }
                foreach (Crop crop in toRemoveList)
                {
                    day.PlantedCrops.Remove(crop);
                }
            }
        }
        public static void swapPlantedCropsDay(ObservableCollection<Day> season)
        {
            ObservableCollection<Crop> toSwap = new ObservableCollection<Crop>();

            foreach (Day day in season)
            {
                foreach (Crop crop in day.CropList)
                {
                    if (day.CropNote.Contains(crop.Name) && crop.IsChecked)
                    {
                        toSwap.Add(crop);
                    }
                }
                foreach (Crop crop in toSwap)
                {
                    currentSelectedDay = day;
                    updateNote(crop, "swapIcon", false);
                }
                toSwap.Clear();
            }
        }
        #endregion
        #region Save changes to XML
        private static void saveSeason(ObservableCollection<Day> season, string seasonName)
        {
            foreach (Day day in season)
            {
                doc.Descendants(seasonName).Descendants("day")
                    .Where(id => id.Attribute("id").Value == day.ID.ToString())
                    .Select(n => n.Attribute("playernote")).FirstOrDefault()
                    .Value = day.PlayerNote;
                doc.Descendants(seasonName).Descendants("day")
                    .Where(id => id.Attribute("id").Value == day.ID.ToString())
                    .Select(n => n.Attribute("cropnote")).FirstOrDefault()
                    .Value = day.CropNote;
            }
        }
        public static void saveCalendarXML()
        {
            foreach (string season in seasons)
            {
                switch (season)
                {
                    case "spring":
                        saveSeason(Spring, season);
                        break;
                    case "summer":
                        saveSeason(Summer, season);
                        break;
                    case "fall":
                        saveSeason(Fall, season);
                        break;
                    case "winter":
                        saveSeason(Winter, season);
                        break;
                    default:
                        break;
                }
            }

            doc.Save(docPath);
        }
        #endregion
    }
    public class Day : INotifyPropertyChanged
    {
        private string _Season;
        private int _ID;
        private string _Event;
        private string _EventName;
        private string _PlayerNote;
        private string _CropNote;
        public ICollectionView CropListView { get; set; }
        private ObservableCollection<Crop> _CropList;
        private ObservableCollection<Crop> _PlantedCrops;
        
        public string Season
        {
            get { return _Season; }
            set { _Season = value; OnPropertyChanged("Season"); }
        }
        public int ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged("ID"); }
        }
        public string Event
        {
            get { return _Event; }
            set { _Event = value; OnPropertyChanged("Event"); }
        }
        public string EventName
        {
            get { return _EventName; }
            set { _EventName = value; OnPropertyChanged("EventName"); }
        }
        public string PlayerNote
        {
            get { return _PlayerNote; }
            set { _PlayerNote = value; OnPropertyChanged("PlayerNote"); }
        }
        public string CropNote
        {
            get { return _CropNote; }
            set { _CropNote = value; OnPropertyChanged("CropNote"); }
        }
        public ObservableCollection<Crop> CropList
        {
            get
            {
                if (_CropList == null)
                {
                    _CropList = new ObservableCollection<Crop>();
                }
                CropListView = CollectionViewSource.GetDefaultView(_CropList);
                return _CropList;
            }
            set
            {
                _CropList = value;
            }
        }
        public ObservableCollection<Crop> PlantedCrops
        {
            get
            {
                if (_PlantedCrops == null)
                {
                    _PlantedCrops = new ObservableCollection<Crop>();
                }
                return _PlantedCrops;
            }
            set
            {
                _PlantedCrops = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Crop : INotifyPropertyChanged
    {
        private string _Name;
        private int _Days;
        private int _Cont;
        private string _Season;
        private string _Image;
        private bool _isChecked;
        private bool _IconOnYield;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        public int Days
        {
            get { return _Days; }
            set { _Days = value; OnPropertyChanged("Days"); }
        }
        public int Cont
        {
            get { return _Cont; }
            set { _Cont = value; OnPropertyChanged("Cont"); }
        }
        public string Season
        {
            get { return _Season; }
            set { _Season = value; OnPropertyChanged("Season"); }
        }
        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged("Image"); }
        }
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; OnPropertyChanged("IsChecked"); }
        }
        public bool IconOnYield
        {
            get { return _IconOnYield; }
            set { _IconOnYield = value; OnPropertyChanged("IconOnYield"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
