using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private User user;
        public startpagina(User login)
        {
            InitializeComponent();
            user = login;
            if (user.Admin == true)

                AdminButton.Visibility = Visibility.Visible;
            else
                AdminButton.Visibility = Visibility.Hidden;

            
        }
        
        private void RapportAanvraagButton_Click(object sender, RoutedEventArgs e)
        {
            var winPatient = new aanmakenPatient(user);
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

       

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var winAdmin = new adminGui(user);
            winAdmin.Show();
            this.Close();
        }

        private void RapportenButton_Click(object sender, RoutedEventArgs e)
        {
            var winRapporten = new OverviewForms(user);
            winRapporten.Show();
            this.Close();
        }

        

        
    }
}
