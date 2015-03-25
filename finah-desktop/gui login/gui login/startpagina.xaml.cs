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

namespace gui_login
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RapportAanvraagButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("tot ziens ");
            var winLogin = new login();
            winLogin.Show();
            this.Close();
        }

        
    }
}
