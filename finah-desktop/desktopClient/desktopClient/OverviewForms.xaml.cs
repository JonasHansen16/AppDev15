using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for OverviewForms.xaml
    /// </summary>
    public partial class OverviewForms : Window
    {
        private User user;
        private int amount;
        private int currentPage;
        private List<Form> _forms;

        public OverviewForms(User currentUser)
        {
            InitializeComponent();
            user = currentUser;
            currentPage = 1;
            GetAmount();
            LoadForms(0);

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            LoadForms(currentPage++);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            LoadForms(currentPage++);
        }

        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winStart = new startpagina(user);
            winStart.Show();
            this.Close();
        }

        private void LoadForms(int page)
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

            List<GridForm> gridForm = new List<GridForm>();
            USST usst = new USST();
            usst.US = user;
            usst.ST = currentPage;
            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetForms"].ConnectionString, usst);
            _forms = response.Content.ReadAsAsync<List<Form>>().Result;

            for(int i = 0; i < _forms.Count; i++)
            {
                GridForm form = new GridForm(_forms[i].Id, _forms[i].Memo, _forms[i].Category, _forms[i].Relation, _forms[i].Completed);
                for (int j = 0; j < _forms[i].ClientList.Count; j++)
                {
                    if (_forms[i].ClientList[j].Function == "Client")
                    {
                        form.C_Leeftijd = _forms[i].ClientList[j].Age;
                        form.C_Ingevuld = _forms[i].ClientList[j].Done;
                    }
                    else
                    {
                        form.M_Leeftijd = _forms[i].ClientList[j].Age;
                        form.M_Ingevuld = _forms[i].ClientList[j].Done;
                    }
                }
                gridForm.Add(form);
                
            }

            formsGrid.ItemsSource = gridForm;

          




        }
        private void GetAmount()
        {
            try
            {

                HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetAmountForms"].ConnectionString, user);
                amount = response.Content.ReadAsAsync<int>().Result;

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database "+ ex);
            }

            if (amount == -1)
            {
                MessageBox.Show("Error Opgetreden Tijdens het zoeken naar openstaande aanvragen");
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

                PageLabel.Content = (currentPage + 1) + " / " + amount;
            }


        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            DataRowView Grdrow = ((FrameworkElement)sender).DataContext as DataRowView;
            int rowIndex = formsGrid.Items.IndexOf(Grdrow);
            

            var window = new vraag(client);
            window.Show();
            this.Close();

        }

        private void formsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((_forms[formsGrid.SelectedIndex].ClientList[0].Function == "Mantelzorger") && (_forms[formsGrid.SelectedIndex].ClientList[0].Done)) || ((_forms[formsGrid.SelectedIndex].ClientList[1].Function == "Mantelzorger") && (_forms[formsGrid.SelectedIndex].ClientList[1].Done)))
                CareStartButton.IsEnabled = false;
            else
                CareStartButton.IsEnabled = true;
            if (((_forms[formsGrid.SelectedIndex].ClientList[0].Function == "Client") && (_forms[formsGrid.SelectedIndex].ClientList[0].Done)) || ((_forms[formsGrid.SelectedIndex].ClientList[1].Function == "Client") && (_forms[formsGrid.SelectedIndex].ClientList[1].Done)))
                ClientStartButton.IsEnabled = false;
            else
                ClientStartButton.IsEnabled = true;
            if (_forms[formsGrid.SelectedIndex].Completed == true)
                ReviewButton.IsEnabled = true;
            else
                ReviewButton.IsEnabled = false;
            
                
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReportGUI(user, _forms[formsGrid.SelectedIndex]);
            window.Show();
            this.Close();

        }

        private void ClientStartButton_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            if (_forms[formsGrid.SelectedIndex].ClientList[0].Function == "Client")
            {
                client = new Client(_forms[formsGrid.SelectedIndex].ClientList[0].Id, _forms[formsGrid.SelectedIndex].ClientList[0].Hash);
            }
            else
            {
                client = new Client(_forms[formsGrid.SelectedIndex].ClientList[1].Id, _forms[formsGrid.SelectedIndex].ClientList[1].Hash);
            }
            var window = new vraag(client);
            window.Show();
            this.Close();

        }

        private void CareStartButton_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            if (_forms[formsGrid.SelectedIndex].ClientList[0].Function == "Mantelzorger")
            {
                client = new Client(_forms[formsGrid.SelectedIndex].ClientList[0].Id, _forms[formsGrid.SelectedIndex].ClientList[0].Hash);
            }
            else
            {
                client = new Client(_forms[formsGrid.SelectedIndex].ClientList[1].Id, _forms[formsGrid.SelectedIndex].ClientList[1].Hash);
            }
            var window = new vraag(client);
            window.Show();
            this.Close();

        }

     

       
    }
}

