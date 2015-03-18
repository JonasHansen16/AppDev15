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
    /// Interaction logic for registreren.xaml
    /// </summary>
    public partial class registreren : Window
    {
        public registreren()
        {
            InitializeComponent();
        }
        private void nummers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void emailvalidation(object sender, TextCompositionEventArgs e)
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

        private void nummers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
            if (char.IsPunctuation(e.Text, e.Text.Length - 1) && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (char.IsSeparator(e.Text, e.Text.Length - 1) && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }



        }

        private void emailvalidation2(object sender, RoutedEventArgs e)
        {
            if (!(textBoxEmail.Text == textBoxEmail2.Text))
            {
                textBoxEmail2.ToolTip = "niet hetzelfde email adres. geeft het juiste email adres in";
            }
        }

        
    }
}
