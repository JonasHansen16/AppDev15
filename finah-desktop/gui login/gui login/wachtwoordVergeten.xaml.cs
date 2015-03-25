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
using System.Text.RegularExpressions;


namespace gui_login
{
    /// <summary>
    /// Interaction logic for wachtwoordVergeten.xaml
    /// </summary>
    public partial class wachtwoordVergeten : Window
    {
        public wachtwoordVergeten()
        {
            InitializeComponent();
        }

        private void emailvalidation(object sender, System.EventArgs e)
        {
            if (textBoxEmail.Text.Length == 0)
            {
                textBoxEmail.ToolTip = "Enter an email.";
                textBoxEmail.Focus();
            }
            else if (!Regex.IsMatch(textBoxEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                textBoxEmail.ToolTip = "Enter a valid email.";
                textBoxEmail.Select(0, textBoxEmail.Text.Length);
                textBoxEmail.Focus();
            }
        }

        private void WachtwoordVerzendenButton_Click(object sender, RoutedEventArgs e)
        {
            //nakijken juiste beroep met gebruikersnaam en email


            if (textBoxEmail.Text.Length!=0 && logintextbox.Text.Length != 0 && beroepTextbox.Text.Length != 0)
            {

MessageBox.Show("uw nieuwe wachtwoord wordt nu verstuurt.");
            }
            else
            {
                if (textBoxEmail.Text.Length ==0)
                {
                    textBoxEmail.ToolTip = "vul uw emailadres in";
                }
                if (logintextbox.Text.Length == 0)
                {
                    logintextbox.ToolTip = "vul uw login in";
                }
                if (beroepTextbox.Text.Length == 0)
                {
                    beroepTextbox.ToolTip = "vul uw beroep in";
                }
            }
            
        }

        
    }
}
