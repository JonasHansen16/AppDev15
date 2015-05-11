using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sprint_1_def
{
    /// <summary>
    /// Interaction logic for adminGui.xaml
    /// </summary>
    public partial class adminGui : Window
    {
        public adminGui()
        {
            InitializeComponent();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            List<Hulpverlener> items = new List<Hulpverlener>();

            items.Add(new Hulpverlener("Pieter Janssen"));
            items.Add(new Hulpverlener("Rita Panissen"));
            items.Add(new Hulpverlener("Jeff Goyen"));
            items.Add(new Hulpverlener("Pieter Post"));
            items.Add(new Hulpverlener("Pieter Janssen"));
            items.Add(new Hulpverlener("Rita Panissen"));
            items.Add(new Hulpverlener("Jeff Goyen"));
            items.Add(new Hulpverlener("Pieter Post"));
            items.Add(new Hulpverlener("Pieter Janssen"));

            items.Add(new Hulpverlener("Pieter Post"));

            var grid = sender as DataGrid;
            grid.ItemsSource = items;

            grid.ScrollIntoView(items[items.Count - 1]);
        }

        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winStart = new startpagina();
            winStart.Show();
            this.Close();
        }

    }

}

