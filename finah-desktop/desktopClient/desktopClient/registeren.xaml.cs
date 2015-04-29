﻿using System;
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
                EmailTextBox1.Background = Brushes.Red;
            }
            else if (!Regex.IsMatch(EmailTextBox1.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                EmailTextBox1.ToolTip = "Enter a valid email.";
                EmailTextBox1.Select(0, EmailTextBox1.Text.Length - 1);
                EmailTextBox1.Background = Brushes.Red;
            }
            else
            {
                EmailTextBox1.Background = Brushes.White;
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

        private void usernamevalidation(object sender, RoutedEventArgs e)
        {
            if (usernameTextbox.Text.Length > 20)
            {
                usernameTextbox.ToolTip = "de username mag maximum 20 tekens lang zijn";
                usernameTextbox.Background = Brushes.Red;
            }else
            if (usernameTextbox.Text.Length < 4)
            {
                usernameTextbox.ToolTip = "de username moet minstens 4 tekens lang zijn";
                usernameTextbox.Background = Brushes.Red;
            }
            else
            {
                usernameTextbox.Background = Brushes.White;
            }

        }

        private void lastnamevalidation(object sender, RoutedEventArgs e)
        {
            if (AchternaamTextBox.Text.Length > 50)
            {
                AchternaamTextBox.ToolTip = "de achternaam mag maximum 50 tekens lang zijn";
                AchternaamTextBox.Background = Brushes.Red;
            }else
            if (AchternaamTextBox.Text.Length < 1)
            {
                AchternaamTextBox.ToolTip = "de achternaam moet minstens 1 teken lang zijn";
                AchternaamTextBox.Background = Brushes.Red;
            }
            else
            {
                AchternaamTextBox.Background = Brushes.White;
            }
        }

        private void firstnamevalidation(object sender, RoutedEventArgs e)
        {
            if (VoornaamTextBox.Text.Length > 50)
            {
                VoornaamTextBox.ToolTip = "de voornaam mag maximum 50 tekens lang zijn";
                VoornaamTextBox.Background = Brushes.Red;
            } else
            if (VoornaamTextBox.Text.Length < 1)
            {
                VoornaamTextBox.ToolTip = "de voornaam moet minstens 1 teken lang zijn";
                VoornaamTextBox.Background = Brushes.Red;
            }
            else
            {
                VoornaamTextBox.Background = Brushes.White;
            }
        }

        private void passwordvalidation(object sender, RoutedEventArgs e)
        {
            if (textboxWachtwoord.Password != null)
            {
                if (textboxWachtwoord.Password.Length < 8)
                {
                    textboxWachtwoord.ToolTip = "het wachtwoord moet minstens 8 tekens lang zijn";
                    textboxWachtwoord.Background = Brushes.Red;
                }
                else
                {
                    textboxWachtwoord.Background = Brushes.White;
                }
            }else
            if (textboxWachtwoord.Password != null && textBoxWachtwoord2.Password != null)
            {
                if (!(textboxWachtwoord.Password == textBoxWachtwoord2.Password))
                {
                    textBoxWachtwoord2.ToolTip = "niet hetzelfde wachtwoord. geeft het juiste wachtwoord in";
                    textBoxWachtwoord2.Background = Brushes.Red;
                }
            }
            else
            {
                textBoxWachtwoord2.Background = Brushes.White;
                textboxWachtwoord.Background = Brushes.White;
            }
        }


        private void aanvraagButton_Click(object sender, RoutedEventArgs e)
        {
            name = VoornaamTextBox.Text;
            username = usernameTextbox.Text;
            lastName = AchternaamTextBox.Text;
            Email = EmailTextBox1.Text;
            password = textboxWachtwoord.Password;
            occupation = beroepTextbox.Text;
            sendRequestWrapper(inputfieldsToUser());
        }

        /// <summary>
        /// This function is a simple wrapper for the sendRequest function that
        /// will call regError() instead of throwing an exception upon 
        /// encountering an error while communicating with the API.
        /// </summary>
        private void sendRequestWrapper(User toRegister)
        {
            try
            {
                sendRequest(toRegister);
            }
            catch (Exception ex)
            {
                regError(ex);
            }
        }

        /// <summary>
        /// This function will send a registration request to the API.
        /// It will call regSuccess() upon successful registration,
        /// it will call regFailure() upon unsuccessful registration,
        /// and it will call throw an exception upon encountering an error while
        /// communicating with the API.
        /// </summary>
        private void sendRequest(User toRegister)
        {
            bool result;

            //Create our client
            using (var client = new HttpClient())
            {
                // Set the base address
                client.BaseAddress = new Uri("http://localhost:18137");
                // Make our request and request the results
                HttpResponseMessage response = client.PostAsJsonAsync("api/user/register", toRegister).Result;
                // Throw an exception if an error occurs
                response.EnsureSuccessStatusCode();
                // Fetch our actual results
                result = response.Content.ReadAsAsync<bool>().Result;
            }

            // We have our result, now do something with it
            if (result)
                regSuccess();
            else
                regFailure(toRegister);

        }

        /// <summary>
        /// Will simply show a messagebox saying that 
        /// the registration was successful.
        /// </summary>
        private void regSuccess()
        {
            MessageBox.Show("Successvol geregistreerd!");
        }

        /// <summary>
        /// Will attempt to find out why the registration failed,
        /// and will display an appropriate message.
        /// </summary>
        private void regFailure(User failure)
        {
            bool exists;

            // We check whether or not the user already existed.
            using (var client = new HttpClient())
            {
                // Set the base address
                client.BaseAddress = new Uri("http://localhost:18137");
                // Make our request and request the results
                HttpResponseMessage response = client.PostAsJsonAsync("api/user/exists", failure).Result;
                // Throw an exception if an error occurs
                response.EnsureSuccessStatusCode();
                // Fetch our actual results
                exists = response.Content.ReadAsAsync<bool>().Result;
            }

            // If the user already existed, we display an appropriate message.
            if (exists)
                MessageBox.Show("Registreren mislukt: gebruikersnaam is al in gebruik.");
            else
                MessageBox.Show("Registreren mislukt.");
        }

        /// <summary>
        /// Will display a MessageBox explaining to the user 
        /// informing them that there was an error while connecting to the API.
        /// </summary>
        /// <param name="ex">The exception that occured.</param>
        private void regError(Exception ex)
        {
            MessageBox.Show("Er is een fout opgetreden tijdens het communiceren met de API. Details: " + ex.Message);
        }

        /// <summary>
        /// Turns all the input fields into an object of type User.
        /// </summary>
        /// <returns>A User created via the input fields.</returns>
        private User inputfieldsToUser()
        {
            MD5 md5hash = MD5.Create();
            User output = new User()
            {
                UserName = username,
                Name = name,
                LastName = lastName,
                Email = Email,
                Password = GetMd5Hash(md5hash, password),
                Occupation = occupation,
                Admin = false,
                Active = false,
                Denied = false
            };

            return output;
        }

        string GetMd5Hash(MD5 md5Hash, string input)
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

