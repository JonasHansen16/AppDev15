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
        List<User> currentUsers = new List<User>();



        public adminGui(User login)
        {
            InitializeComponent();
            user = login;
            sendGetAmountUsersRequest();
            LoadUsers(0);
            usergrid.IsReadOnly = true;
        }



        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winStart = new startpagina(user);
            winStart.Show();
            this.Close();
        }

        private void sendGetAmountUsersRequest()
        {
            int pages;
            try
            {

                HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetAmountInactiveUsers"].ConnectionString, user);
                amount = response.Content.ReadAsAsync<int>().Result;

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database " + ex);
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
                    pages = amount / 10;
                }
                else
                {
                    pages = amount / 10 + 1;
                }

                if (pages < currentPage)
                    currentPage = currentPage - 1;

                PageLabel.Content = (currentPage + 1) + " / " + pages;


            }

        }



        private void LoadUsers(int page)
        {
            currentPage = page;
            if (currentPage == 0)
                PreviousButton.IsEnabled = false;
            else
                PreviousButton.IsEnabled = true;

            if ((currentPage+1) >= (double)((amount / 10.0 )))
                NextButton.IsEnabled = false;
            else
                NextButton.IsEnabled = true;
            List<GridUser> userGridList = new List<GridUser>();
            ADMST admst = new ADMST();
            admst.ADM = user;
            admst.ST = currentPage * 10;
            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetInactiveUsers"].ConnectionString, admst);
            _allusers = response.Content.ReadAsAsync<List<User>>().Result;
            for (int i = 0; i < _allusers.Count; i++)
            {
                GridUser gridUser = new GridUser(_allusers[i].Name, _allusers[i].LastName, _allusers[i].Email, _allusers[i].Occupation, _allusers[i].UserName, _allusers[i].Id);
                userGridList.Add(gridUser);
            }


            usergrid.ItemsSource = userGridList;
            sendGetAmountUsersRequest();



        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
           currentUsers = _allusers;

            LoadUsers(++currentPage);
            for (int i = 0; i < currentUsers.Count; i++)
            {
                for (int j = 0; j < _allusers.Count; j++)
                {
                    if (CompareUsers(currentUsers[i], _allusers[j]))
                    {
                        _allusers.RemoveAt(j);
                        break;
                    }

                }


            }
            List<GridUser> userGridList = new List<GridUser>();
            for (int i = 0; i < _allusers.Count; i++)
            {
                GridUser gridUser = new GridUser(_allusers[i].Name, _allusers[i].LastName, _allusers[i].Email, _allusers[i].Occupation, _allusers[i].UserName, _allusers[i].Id);
                userGridList.Add(gridUser);
            }


            usergrid.ItemsSource = userGridList;




        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            currentUsers.Clear();
            LoadUsers(--currentPage);
            
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
                {

                    sendGetAmountUsersRequest();
                    LoadUsers(currentPage);
                    for (int i = 0; i < currentUsers.Count; i++)
                    {
                        for (int j = 0; j < _allusers.Count; j++)
                        {
                            if (CompareUsers(currentUsers[i], _allusers[j]))
                            {
                                _allusers.RemoveAt(j);
                                break;
                            }

                        }


                    }
                    List<GridUser> userGridList = new List<GridUser>();
                    for (int i = 0; i < _allusers.Count; i++)
                    {
                        GridUser gridUser = new GridUser(_allusers[i].Name, _allusers[i].LastName, _allusers[i].Email, _allusers[i].Occupation, _allusers[i].UserName, _allusers[i].Id);
                        userGridList.Add(gridUser);
                    }
                    usergrid.ItemsSource = userGridList;
                }
                else
                    MessageBox.Show("Error Opgetreden tijdens activeren, probeer het later opnieuw");

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database " + ex);
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

                HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["SetUserDeny"].ConnectionString, admus);
                result = response.Content.ReadAsAsync<bool>().Result;

                if (result == true)
                {
                    sendGetAmountUsersRequest();
                    LoadUsers(currentPage);
                    for (int i = 0; i < currentUsers.Count; i++)
                    {
                        for (int j = 0; j < _allusers.Count; j++)
                        {
                            if (CompareUsers(currentUsers[i], _allusers[j]))
                            {
                                _allusers.RemoveAt(j);
                                break;
                            }

                        }


                    }
                    List<GridUser> userGridList = new List<GridUser>();
                    for (int i = 0; i < _allusers.Count; i++)
                    {
                        GridUser gridUser = new GridUser(_allusers[i].Name, _allusers[i].LastName, _allusers[i].Email, _allusers[i].Occupation, _allusers[i].UserName, _allusers[i].Id);
                        userGridList.Add(gridUser);
                    }
                    usergrid.ItemsSource = userGridList;
                }
                else
                    MessageBox.Show("Error opgetreden tijdens weigeren van de user, probeer het later opnieuw");

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database " + ex);
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

        private bool CompareUsers(User user1, User user2)
        {
            if (user1.Id == user2.Id)
                return true;

            else
                return false;
        }




    }
}

