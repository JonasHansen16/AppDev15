﻿using System;
using System.Collections.Generic;
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
            User loginUser = new User();
            loginUser.UserName = logintextbox.Text;
            loginUser.Password = HashMd5.Md5Hash(PasswordTextBox.Password);
            sendRequestWrapper(loginUser);


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vergetenStart = new wachtwoordVergeten();
            vergetenStart.Show();
            this.Close();
        }

        /// <summary>
        /// This function is a simple wrapper for the sendRequest function that
        /// will call regError() instead of throwing an exception upon 
        /// encountering an error while communicating with the API.
        /// </summary>
        private void sendRequestWrapper(User toLogin)
        {
            try
            {
                sendRequest(toLogin);
            }
            catch (Exception ex)
            {
                LoginError(ex);
            }
        }

        /// <summary>
        /// This function will send a registration request to the API.
        /// It will call regSuccess() upon successful registration,
        /// it will call regFailure() upon unsuccessful registration,
        /// and it will call throw an exception upon encountering an error while
        /// communicating with the API.
        /// </summary>
        private void sendRequest(User toLogin)
        {
            User result;

            //Create our client
            using (var client = ApiConnection.getConnection())
            {

                // Make our request and request the results
                HttpResponseMessage response = client.PostAsJsonAsync("api/user/Login", toLogin).Result;
                // Throw an exception if an error occurs
                response.EnsureSuccessStatusCode();
                // Fetch our actual results
                result = response.Content.ReadAsAsync<User>().Result;
            }

            // We have our result, now do something with it
            if (result != null)
                LoginSuccess();
            else
                LoginFailure(toLogin);

        }

        /// <summary>
        /// Will simply show a messagebox saying that 
        /// the registration was successful.
        /// </summary>
        private void LoginSuccess()
        {
            MessageBox.Show("Successvol Ingelogd");
            var winStart = new startpagina();
            winStart.Show();
            this.Close();

        }

        /// <summary>
        /// Will attempt to find out why the registration failed,
        /// and will display an appropriate message.
        /// </summary>
        private void LoginFailure(User failure)
        {
            bool exists;

            // We check whether or not the user already existed.
            using (var client = ApiConnection.getConnection())
            {

                // Make our request and request the results

                HttpResponseMessage response = client.PostAsJsonAsync("api/user/exists", failure).Result;
                // Throw an exception if an error occurs
                response.EnsureSuccessStatusCode();
                // Fetch our actual results
                exists = response.Content.ReadAsAsync<bool>().Result;
            }

            // If the user already existed, we display an appropriate message.
            if (exists)
                UserStatus(failure);
            else
                MessageBox.Show("Inloggen mislukt username bestaat niet.");
        }

        /// <summary>
        /// Will display a MessageBox explaining to the user 
        /// informing them that there was an error while connecting to the API.
        /// </summary>
        /// <param name="ex">The exception that occured.</param>
        private void LoginError(Exception ex)
        {
            MessageBox.Show("Er is een fout opgetreden tijdens het communiceren met de API. Details: " + ex.Message);
        }

        private void UserStatus(User status)
        {
            bool denied;
            using (var client = ApiConnection.getConnection())
            {


                // Make our request and request the results

                HttpResponseMessage response = client.PostAsJsonAsync("api/user/Denied", status).Result;
                // Throw an exception if an error occurs
                response.EnsureSuccessStatusCode();
                // Fetch our actual results
                denied = response.Content.ReadAsAsync<bool>().Result;
            }

            if (denied)

                MessageBox.Show("Inloggen mislukt gebruiker is denied");
            else
                UserActive(status);



        }

        private void UserActive(User status)
        {
            bool active;
            using (var client = ApiConnection.getConnection())
            {


                // Make our request and request the results

                HttpResponseMessage response = client.PostAsJsonAsync("api/user/Active", status).Result;
                // Throw an exception if an error occurs
                response.EnsureSuccessStatusCode();
                // Fetch our actual results
                active = response.Content.ReadAsAsync<bool>().Result;
            }

            if (active)
                MessageBox.Show("Gebruikersnaam en passwoord komen niet overeen.");
            else
                MessageBox.Show("Gebruiker is nog niet goedgekeurd.");

        }
    }
}
