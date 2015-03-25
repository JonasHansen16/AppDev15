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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Administrator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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

    }

    class Hulpverlener
    {
        public string Name { get; set; }

        public string Details { get; set; }

        public Hulpverlener(string name)
        {
            this.Name = name;
            this.Details = "Bekijk details";
        }
    }
}
