using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public class FarmManager
    {
        private static ObservableCollection<int> _resolutions;
        public static ObservableCollection<int> Resolutions
        {
            get
            {
                if (_resolutions == null)
                {
                    _resolutions = new ObservableCollection<int>();
                }
                return _resolutions;
            }
            set { _resolutions = value; }
        }
        private static string _farmLayout;
        public static string FarmLayout
        {
            get { return _farmLayout; }
            set { _farmLayout = value; }
        }
        private static ObservableCollection<Layout> _farmLayouts;
        public static ObservableCollection<Layout> FarmLayouts
        {
            get
            {
                if (_farmLayouts == null)
                {
                    _farmLayouts = new ObservableCollection<Layout>();
                }
                return _farmLayouts;
            }
            set { _farmLayouts = value; }
        }
        private static ObservableCollection<Terrain> _TerrainObjects;
        public static ObservableCollection<Terrain> TerrainObjects
        {
            get
            {
                if (_TerrainObjects == null)
                {
                    _TerrainObjects = new ObservableCollection<Terrain>();
                }
                return _TerrainObjects;
            }
            set { _TerrainObjects = value; }
        }
        private static Terrain _SelectedTerrain;
        public static Terrain SelectedTerrain
        {
            get { return _SelectedTerrain; }
            set { _SelectedTerrain = value; }
        }

        public static void loadFarmInformation()
        {
            #region Farm Layouts
            FarmLayouts.Add(new Layout { Name = "Standard", Image = "images/farms/standard.jpg", Icon = "images/farms/standardIcon.png" });
            FarmLayouts.Add(new Layout { Name = "Riverland", Image = "images/farms/riverland.jpg", Icon = "images/farms/riverlandIcon.png" });
            FarmLayouts.Add(new Layout { Name = "Forest", Image = "images/farms/forest.jpg", Icon = "images/farms/forestIcon.png" });
            FarmLayouts.Add(new Layout { Name = "Hill-Top", Image = "images/farms/hilltop.jpg", Icon = "images/farms/hilltopIcon.png" });
            FarmLayouts.Add(new Layout { Name = "Wilderness", Image = "images/farms/wilderness.jpg", Icon = "images/farms/wildernessIcon.png" });
            #endregion
            TerrainObjects.Add(new Terrain { Name = "Grass", Image = "images/terrain/grass.png" });
            TerrainObjects.Add(new Terrain { Name = "Amaranth", Image = "images/shippeditems/Amaranth.png" });
            Resolutions.Add(50);
            Resolutions.Add(75);
            Resolutions.Add(100);
        }

        public static Layout getLayout(string layoutName)
        {
            return FarmLayouts.Where(n => n.Name == layoutName).FirstOrDefault();
        }
    }
    public class Layout
    {
        private string _Name;
        private string _Image;
        private string _Icon;

        public string Image
        {
            get { return _Image; }
            set { _Image = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }
    }
    public class Terrain
    {
        private string _Name;
        private string _Image;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Image
        {
            get { return _Image; }
            set { _Image = value; }
        }
    }
}
