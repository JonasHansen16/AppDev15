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

namespace sprint_1_def
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class login : Window
    {
        public login()
        {
            InitializeComponent();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            var winRegisteren = new registeren();
            winRegisteren.Show();
            this.Close();
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Welkom " + logintextbox.Text);
            var winStart = new startpagina();
            winStart.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vergetenStart = new wachtwoordVergeten();
            vergetenStart.Show();
            this.Close();
        }
    }
}
