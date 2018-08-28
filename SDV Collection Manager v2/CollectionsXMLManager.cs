using System.Xml.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SDV_Collection_Manager_v2.Commands;
using System.Windows.Data;

namespace Managers
{
    public class CollectionsXMLManager
    {
        #region Collections
        #region ShippedItems
        ICollectionView _PageOneCollectionView { get; set; }
        private static ObservableCollection<Item> pageonecollection;
        public ObservableCollection<Item> PageOneCollection
        {
            get
            {
                if (pageonecollection == null)
                {
                    pageonecollection = new ObservableCollection<Item>();
                }
                _PageOneCollectionView = CollectionViewSource.GetDefaultView(pageonecollection);
                return pageonecollection;
            }
            set { pageonecollection = value; }
        }
        ICollectionView _PageTwoCollectionView { get; set; }
        private static ObservableCollection<Item> pagetwocollection;
        public ObservableCollection<Item> PageTwoCollection
        {
            get
            {
                if (pagetwocollection == null)
                {
                    pagetwocollection = new ObservableCollection<Item>();
                }
                _PageTwoCollectionView = CollectionViewSource.GetDefaultView(pagetwocollection);
                return pagetwocollection;
            }
            set { pagetwocollection = value; }
        }
        #endregion
        #region Fishing
        ICollectionView _FishCollectionView { get; set; }
        private static ObservableCollection<Item> fishcollection;
        public ObservableCollection<Item> FishCollection
        {
            get
            {
                if (fishcollection == null)
                {
                    fishcollection = new ObservableCollection<Item>();
                }
                _FishCollectionView = CollectionViewSource.GetDefaultView(fishcollection);
                return fishcollection;
            }
            set { fishcollection = value; }
        }
        #endregion
        #region Artifacts
        ICollectionView _ArtifactCollectionView { get; set; }
        private static ObservableCollection<Item> artifactcollection;
        public ObservableCollection<Item> ArtifactCollection
        {
            get
            {
                if (artifactcollection == null)
                {
                    artifactcollection = new ObservableCollection<Item>();
                }
                _ArtifactCollectionView = CollectionViewSource.GetDefaultView(artifactcollection);
                return artifactcollection;
            }
            set { artifactcollection = value; }
        }
        #endregion
        #region Minerals
        ICollectionView _MineralCollectionView { get; set; }
        private static ObservableCollection<Item> mineralcollection;
        public ObservableCollection<Item> MineralCollection
        {
            get
            {
                if (mineralcollection == null)
                {
                    mineralcollection = new ObservableCollection<Item>();
                }
                _MineralCollectionView = CollectionViewSource.GetDefaultView(mineralcollection);
                return mineralcollection;
            }
            set { mineralcollection = value; }
        }
        #endregion
        #region Cooking
        ICollectionView _CookingCollectionView { get; set; }
        private static ObservableCollection<Item> cookingcollection;
        public ObservableCollection<Item> CookingCollection
        {
            get
            {
                if (cookingcollection == null)
                {
                    cookingcollection = new ObservableCollection<Item>();
                }
                _CookingCollectionView = CollectionViewSource.GetDefaultView(cookingcollection);
                return cookingcollection;
            }
            set { cookingcollection = value; }
        }
        #endregion
        string[] collections = new string[] { "pageone", "pagetwo", "fish", "artifacts", "minerals", "cooking" };
        #endregion
        #region Filter methods & properties
        private bool ItemFilter(object obj)
        {
            Item item = obj as Item;
            string tempStatus;

            if (Search != null)
            {
                #region Check for optional Status search words
                if (Search.ToLower().Contains("owned") || Search.ToLower().Contains("acquired") || Search.ToLower().Contains("have") || Search.ToLower().Contains("true"))
                {
                    tempStatus = "true";
                }
                else if (Search.ToLower().Contains("missing") || Search.ToLower().Contains("false"))
                {
                    tempStatus = "false";
                }
                else
                {
                    tempStatus = Search;
                }
                #endregion
                
                if (item.Name.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.Status.ToString().IndexOf(tempStatus, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.Season.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.Time.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.Weather.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.Source.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        private string _search { get; set; }
        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                OnSearchChanged();
            }
        }
        private void OnSearchChanged()
        {
            _PageOneCollectionView.Filter = ItemFilter;
            _PageTwoCollectionView.Filter = ItemFilter;
            _FishCollectionView.Filter = ItemFilter;
            _ArtifactCollectionView.Filter = ItemFilter;
            _MineralCollectionView.Filter = ItemFilter;
            _CookingCollectionView.Filter = ItemFilter;
        }
        public string searchTooltip { get { return "Searches for an item that contains the inserted value.\nThis value can be an item's Name, Status, Season, Weather, Time or Source."; } }
        #endregion
        XDocument doc = new XDocument();
        string docPath = @"assets\Collections.xml";
        public UpdateItem _updateItem { get; set; }
        public OpenWebpage _openWebpage { get; set; }

        public CollectionsXMLManager()
        {
            _updateItem = new UpdateItem(this);
            _openWebpage = new OpenWebpage(this);
        }

        private Item newItem(XElement e, string collection)
        {
            Item item =  new Item {
                Status = Boolean.Parse(e.Attribute("status").Value),
                Name = e.Attribute("name").Value,
                Href = e.Attribute("href").Value,
                Img = "images/"+collection+e.Attribute("img").Value,
                Time = e.Attribute("time").Value,
                Season = e.Attribute("season").Value,
                Weather = e.Attribute("weather").Value,
                Source = e.Attribute("source").Value,
            };
            return item;
        }

        public void loadCollectionsXML()
        {
            doc = XDocument.Load(docPath);
            foreach (var collection in collections)
            {
                foreach (XElement e in doc.Root.Descendants(collection).Descendants("item"))
                {
                    switch (collection)
                    {
                        case "pageone":
                            PageOneCollection.Add(newItem(e, "shippeditems"));
                            break;
                        case "pagetwo":
                            PageTwoCollection.Add(newItem(e, "shippeditems"));
                            break;
                        case "fish":
                            FishCollection.Add(newItem(e, collection));
                            break;
                        case "artifacts":
                            ArtifactCollection.Add(newItem(e, collection));
                            break;
                        case "minerals":
                            MineralCollection.Add(newItem(e, collection));
                            break;
                        case "cooking":
                            CookingCollection.Add(newItem(e, collection));
                            break;
                    }
                }
            }

        }
        // Run when left-clicking an Image with an Item
        public void updateItemStatus(Item item)
        {
            if (item.Status)
            { item.Status = false; }
            else
            { item.Status = true; }
            #region Refresh CollectionViews
            _PageOneCollectionView.Refresh();
            _PageTwoCollectionView.Refresh();
            _FishCollectionView.Refresh();
            _ArtifactCollectionView.Refresh();
            _MineralCollectionView.Refresh();
            _CookingCollectionView.Refresh();
            #endregion
        }
        // Run when right-clicking an Image with an Item
        public void openWebpage(Item item)
        {
            System.Diagnostics.Process.Start(@"http://stardewvalleywiki.com" + item.Href);
        }

        #region Save changes to XML
        private XAttribute getItemAttribute(string itemName, string collection)
        {
            return doc.Root.Descendants(collection).Descendants("item").Where(n => n.Attribute("name").Value == itemName).Select(s => s.Attribute("status")).FirstOrDefault();
        }
        
        private void replaceAttribute(ObservableCollection<Item> collection, string collectionName)
        {
            foreach (var item in collection)
            {
                getItemAttribute(item.Name, collectionName).Value = item.Status.ToString().ToLower();
            }
        }

        public void saveCollectionXML()
        {
            foreach (var collection in collections)
            {
                switch (collection)
                {
                    case "pageone":
                        replaceAttribute(PageOneCollection, collection);
                        break;
                    case "pagetwo":
                        replaceAttribute(PageTwoCollection, collection);
                        break;
                    case "fish":
                        replaceAttribute(FishCollection, collection);
                        break;
                    case "artifacts":
                        replaceAttribute(ArtifactCollection, collection);
                        break;
                    case "minerals":
                        replaceAttribute(MineralCollection, collection);
                        break;
                    case "cooking":
                        replaceAttribute(CookingCollection, collection);
                        break;
                    default:
                        break;
                }
            }
            doc.Save(docPath);
        }
        #endregion

    }
    public class Item : INotifyPropertyChanged
    {
        public bool _Status;
        public string _Name;
        public string _Href;
        public string _Img;
        public string _Time;
        public string _Season;
        public string _Weather;
        public string _Source;

        public bool Status { get { return _Status; } set { _Status = value; OnPropertyChange("Status"); } }
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChange("Name"); } }
        public string Href { get { return _Href; } set { _Href = value; OnPropertyChange("Href"); } }
        public string Img { get { return _Img; } set { _Img = value; OnPropertyChange("Img"); } }
        public string Time { get { return _Time; } set { _Time = value; OnPropertyChange("Time"); } }
        public string Season { get { return _Season; } set { _Season = value; OnPropertyChange("Season"); } }
        public string Weather { get { return _Weather; } set { _Weather = value; OnPropertyChange("Weather"); } }
        public string Source { get { return _Source; } set { _Source = value; OnPropertyChange("Source"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
