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
using System.Net.Http;
using System.Net.Http.Headers;

namespace sprint_1_def
{
    /// <summary>
    /// Interaction logic for registeren.xaml
    /// </summary>
    public partial class registeren : Window
    {
        static string name;
        static string lastName;
        static string Email;
        static string password;
        static string occupation;
        public registeren()
        {
            InitializeComponent();

        }

        private void emailvalidation(object sender, System.EventArgs e)
        {
            if (EmailTextBox1.Text.Length == 0)
            {
                EmailTextBox1.ToolTip = "Enter an email.";
                EmailTextBox1.Focus();
            }
            else if (!Regex.IsMatch(EmailTextBox1.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                EmailTextBox1.ToolTip = "Enter a valid email.";
                EmailTextBox1.Select(0, EmailTextBox1.Text.Length - 1);
                EmailTextBox1.Focus();
            }
        }

        private void nummers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void nummers_PreviewTextInputTweede(object sender, TextCompositionEventArgs e)
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
            if (!(EmailTextBox1.Text == EmailTextBox2.Text))
            {
                EmailTextBox2.ToolTip = "niet hetzelfde email adres. geeft het juiste email adres in";
            }
        }

        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winLogin = new login();
            winLogin.Show();
            this.Close();
        }

        private void aanvraagButton_Click(object sender, RoutedEventArgs e)
        {
            name = VoornaamTextBox.Text;
            lastName = AchternaamTextBox.Text;
            Email = EmailTextBox1.Text;
            password = PassWordTextBox1.Text;
            occupation = beroepTextbox.Text;
           
        }
        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:9000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var gizmo = new User()
                {
                    Name = name,
                    LastName = lastName,
                    Email = Email,
                    Password = password,
                    Occupation = occupation,
                    Admin = false,
                    Active = false,
                    Denied = false
                };
                HttpResponseMessage response = await client.PostAsJsonAsync("api/User/Register", gizmo);
            }

        }


    }
}

