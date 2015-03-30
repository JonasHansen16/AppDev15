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
    /// Interaction logic for startpagina.xaml
    /// </summary>
    public partial class startpagina : Window
    {
        public startpagina()
        {
            InitializeComponent();
        }
        
        private void RapportAanvraagButton_Click(object sender, RoutedEventArgs e)
        {
            var winPatient = new aanmakenPatient();
            winPatient.Show();
            this.Close();
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("tot ziens ");
            var winLogin = new login();
            winLogin.Show();
            this.Close();
        }

        private void VragenlijstButton_Click(object sender, RoutedEventArgs e)
        {
            var winVraag = new vraag();
            winVraag.Show();
            this.Close();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
var winAdmin = new adminGui();
            winAdmin.Show();
            this.Close();
        }

        private void RapportenButton_Click(object sender, RoutedEventArgs e)
        {
            var winRapporten = new rapportenoverzicht();
            winRapporten.Show();
            this.Close();
        }

        
    }
}
