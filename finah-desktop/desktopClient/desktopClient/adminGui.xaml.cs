using System;
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
using System.Windows.Shapes;

namespace sprint_1_def
{
    /// <summary>
    /// Interaction logic for adminGui.xaml
    /// </summary>
    public partial class adminGui : Window
    {
        private User user;
        private int amount;
        private int currentPage;
        private List<User> _allusers = new List<User>();
        private int selected;


        public adminGui(User login)
        {
            InitializeComponent();
            user = login;
            sendGetAmountUsersRequest();
            LoadUsers(0);
        }

        

        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winStart = new startpagina(user);
            winStart.Show();
            this.Close();
        }

        private void sendGetAmountUsersRequest()
        {
            try
            {

                HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetAmountInactiveUsers"].ConnectionString, user);
                amount = response.Content.ReadAsAsync<int>().Result;
               
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database");
            }

            if (amount == -1)
            {
                MessageBox.Show("User is geen administrator");
                var window = new startpagina(user);
                window.Show();
                this.Close();

            }
            else
            {
                if (amount % 10 == 0)
                {
                    int pages = amount / 10;
                }
                else
                {
                    int pages = amount / 10 + 1;
                }

                PageLabel.Content = (currentPage+1) + " / " + amount;
                
                
            }

        }

     

        private void LoadUsers(int page)
        {
            currentPage = page;
            if (currentPage == 0)
                PreviousButton.IsEnabled = false;
            else
                PreviousButton.IsEnabled = true;

            if (currentPage == amount)
                NextButton.IsEnabled = false;
            else
                NextButton.IsEnabled = true;
            
            ADMST admst = new ADMST();
            admst.ADM = user;
            admst.ST = currentPage * 10;
            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetInactiveUsers"].ConnectionString, admst);
            _allusers = response.Content.ReadAsAsync<List<User>>().Result;

            usergrid.ItemsSource = _allusers;


            
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers(currentPage++);

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers(currentPage--);
        }

        private void sendActivateRequest()
        {

            ADMUS admus = new ADMUS();
            admus.ADM = user;
            admus.US = _allusers[usergrid.SelectedIndex];
            bool result;
            try
            {


                HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["SetUserActive"].ConnectionString, admus);
                result = response.Content.ReadAsAsync<bool>().Result;

                if (result == true)
                    LoadUsers(currentPage);
                else
                    MessageBox.Show("Error Opgetreden tijdens activeren, probeer het later opnieuw");

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database");
            }

           

        }

        private void sendDenyRequest()
        {
            ADMUS admus = new ADMUS();
            admus.US = _allusers[usergrid.SelectedIndex];
            admus.ADM = user;
            bool result;
            try
            {

                HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["SetUserDeny"].ConnectionString, user);
                result = response.Content.ReadAsAsync<bool>().Result;

                if (result == true)
                    LoadUsers(currentPage);
                else
                    MessageBox.Show("Error opgetreden tijdens weigeren van de user, probeer het later opnieuw");

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database");
            }

        }

      
        private void activeerbutton_Click(object sender, RoutedEventArgs e)
        {
            sendActivateRequest();
            
        }

        private void weigerbutton_Click(object sender, RoutedEventArgs e)
        {
            sendDenyRequest();
        }


        

     

    }
}

