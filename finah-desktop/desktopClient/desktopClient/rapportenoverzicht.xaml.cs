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
    /// Interaction logic for rapportenoverzicht.xaml
    /// </summary>
    public partial class rapportenoverzicht : Window
    {
        public rapportenoverzicht()
        {
            InitializeComponent();
        }

        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winStart = new startpagina();
            winStart.Show();
            this.Close();
        }

        private void CreatePatient_Click(object sender, RoutedEventArgs e)
        {
            var winPatient = new aanmakenPatient();
            winPatient.Show();
            this.Close();
        }
    }
}
