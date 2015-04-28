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
using System.Security.Cryptography;

namespace sprint_1_def
{
    /// <summary>
    /// Interaction logic for registeren.xaml
    /// </summary>
    public partial class registeren : Window
    {
        static string name;
        static string username;
        static string lastName;
        static string Email;
        static string password;
        static string occupation;
        public registeren()
        {
            InitializeComponent();

        }
        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winLogin = new login();
            winLogin.Show();
            this.Close();
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

        private void passwordvalidation(object sender, RoutedEventArgs e)
        {
            if (textboxWachtwoord.Text != null && textBoxWachtwoord2.Text == null)
            {
                if (textboxWachtwoord.Text.Length < 8)
                {
                    textboxWachtwoord.ToolTip = "het wachtwoord moet minstens 8 tekens lang zijn";
                    textboxWachtwoord.Background = Brushes.Red;
                }
            }
            if (textboxWachtwoord.Text != null && textBoxWachtwoord2.Text != null)
            {
                if (!(textboxWachtwoord.Text == textBoxWachtwoord2.Text))
                {
                    textBoxWachtwoord2.ToolTip = "niet hetzelfde wachtwoord. geeft het juiste wachtwoord in";
                    textBoxWachtwoord2.Background = Brushes.Red;
                }
            }
        }

        private void aanvraagButton_Click(object sender, RoutedEventArgs e)
        {
            name = VoornaamTextBox.Text;
            username = usernameTextbox.Text;
            lastName = AchternaamTextBox.Text;
            Email = EmailTextBox1.Text;
            password = textboxWachtwoord.Text;
            occupation = beroepTextbox.Text;
            RunAsync().Wait();
           
        }
        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                MD5 md5hash = MD5.Create();
                client.BaseAddress = new Uri("http://localhost:18137/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var gizmo = new User()
                {
                    
                    UserName = username,
                    Name = name,
                    LastName = lastName,
                    Email = Email,
                    Password = GetMd5Hash(md5hash,password),
                    Occupation = occupation,
                    Admin = false,
                    Active = false,
                    Denied = false
                };
                HttpResponseMessage response = await client.PostAsJsonAsync("api/User/Register", gizmo);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(response.Content.ToString());
                }
            }

        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        



    }
}

