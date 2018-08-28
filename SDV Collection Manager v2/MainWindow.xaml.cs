using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using Managers;
using System.Windows.Input;
using System.Text.RegularExpressions;
using WpfAutoGrid;
using System.Windows.Media.Imaging;

namespace SDV_Collection_Manager_v2
{
    public partial class MainWindow : Window
    {
        CollectionsXMLManager collectionsXML = new CollectionsXMLManager();
        FarmPlanner farmPlanner;

        public MainWindow()
        {
            collectionsXML.loadCollectionsXML();
            CalendarManager.buildCalendar();
            FarmManager.loadFarmInformation();
            OptionsMenu.loadConfig();
            InitializeComponent();
            farmPlanner = new FarmPlanner();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OptionsMenu.saveConfig();
            collectionsXML.saveCollectionXML();
            CalendarManager.saveCalendarXML();
            farmPlanner.Close();
            Environment.Exit(0);
        }
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        // Opens Farm Planner window
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            farmPlanner.Show();
        }
        // Opens Stardew Valley Planner by Henrik Peinar
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://stardew.info/planner/");
        }

        private void ShippedItems_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (PageOne.IsSelected)
            {
                PageTwo.IsSelected = true;
            }
            else
            {
                PageOne.IsSelected = true;
            }
        }

        private void TabItem_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (CalendarSeasons.SelectedIndex < CalendarSeasons.Items.Count - 1)
                {
                    CalendarSeasons.SelectedIndex += 1;
                }
                else
                {
                    CalendarSeasons.SelectedIndex = 0;
                }
            }
            else
            {
                if (CalendarSeasons.SelectedIndex > 0)
                {
                    CalendarSeasons.SelectedIndex -= 1;
                }
                else
                {
                    CalendarSeasons.SelectedIndex = CalendarSeasons.Items.Count-1;
                }
            }
        }

        private void CloseManager_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeManager_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
