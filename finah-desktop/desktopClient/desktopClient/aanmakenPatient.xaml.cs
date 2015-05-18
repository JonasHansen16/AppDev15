using nah_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for aanmakenPatient.xaml
    /// </summary>
    public partial class aanmakenPatient : Window
    {

        private User user;

        public aanmakenPatient(User login)
        {
            InitializeComponent();
            user = login;
            InvullenDropdownListVraagenlijsten(user);

        }



        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winStart = new startpagina(user);
            winStart.Show();
            this.Close();
        }

        private void Rapporten()
        {
            var winRapporten = new rapportenoverzicht(user);
            winRapporten.Show();
            this.Close();
        }

        private void InvullenDropdownListVraagenlijsten(User user)
        {
            String VragenlijstenNaam;
            List<Questionnaire> Vragenlijsten = new List<Questionnaire>();
            //api opvragen lijsten
            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetQuestionaires"].ConnectionString, user);
            Vragenlijsten = response.Content.ReadAsAsync<List<Questionnaire>>().Result;

            //questionnaire oproepen
            if (Vragenlijsten != null)
            {
                foreach (Questionnaire item in Vragenlijsten)
                {
                    MessageBox.Show("ok ontvangen");
                    VragenlijstenNaam = item.Title;
                    VragenlijstDropDown.Items.Add(VragenlijstenNaam);
                    MessageBox.Show("afgerond");
                }
            }
            else
            {
                MessageBox.Show("er is een fout opgetreden");
            }

        }
        private String ControlerenAllesIngevuld()
        {
            if (CommentTextBox.Text.Length == 0 || RelationTextBox.Text.Length == 0 || CategoryTextBox.Text.Length == 0 || AgeCareComboBox.SelectedValue == "" || AgeClientComboBox.SelectedValue == "")
            {
                return "NIET OK";
            }
            else
            {
                return "OK";
            }
        }

        private void DoorsturenNaarAPI()
        {
            string ageCareTaker = AgeCareComboBox.SelectedValue.ToString();
            string ageClient = AgeClientComboBox.SelectedValue.ToString();
            string comment = CommentTextBox.Text;
            string relation = RelationTextBox.Text;
            string category = CategoryTextBox.Text;
            string questionnaire = VragenlijstDropDown.SelectedValue.ToString();

            ClientExp CareTaker = new ClientExp(0, "", 20, false, false, "CareTaker");
            ClientExp Client = new ClientExp(0, "", 30, false, false, "Client");
            List<ClientExp> personen = new List<ClientExp>();
            personen.Add(CareTaker);
            personen.Add(Client);
            Form sentForm = new Form(0, comment, category, relation, false, false, personen);
            User sentUser = this.user;
            Questionnaire sentQuestionnaire = new Questionnaire(0, questionnaire, "", "");
            USFOQU sent = new USFOQU(sentUser, sentForm, sentQuestionnaire);
            sendRequestWrapper(sent);
        }

        private void sendRequestWrapper(USFOQU FormToAdd)
        {
            try
            {
                sendRequest(FormToAdd);
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
        private void sendRequest(USFOQU FormToAdd)
        {
            bool result;

            //Create our client
            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["SentForm"].ConnectionString, FormToAdd);
            result = response.Content.ReadAsAsync<bool>().Result;

            // We have our result, now do something with it
            if (result)
            {
                regSuccess();
                Rapporten();

            }
            else
            {
                MessageBox.Show("onbekende fout opgetreden");
            }

        }

        /// <summary>
        /// Will simply show a messagebox saying that 
        /// the registration was successful.
        /// </summary>
        private void regSuccess()
        {
            MessageBox.Show("Successvol toegevoegd");
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

        private void ToevoegenButton_Click(object sender, RoutedEventArgs e)
        {
            string controle = ControlerenAllesIngevuld();
            if (controle == "OK")
            {
                DoorsturenNaarAPI();
            }
            else
            {
                MessageBox.Show("gelieve alles gegevens in te vullen");
            }
        }



        private void CommentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CommentTextBox.Text.Length > 100)
            {
                CommentTextBox.ToolTip = "De tekst mag maximaal 100 tekens lang zijn";
                CommentTextBox.Background = Brushes.Red;
            }
            else
            {
                CommentTextBox.Background = Brushes.White;
            }
        }

        private void RelationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RelationTextBox.Text.Length > 100)
            {
                RelationTextBox.ToolTip = "De tekst mag maximaal 100 tekens lang zijn";
                RelationTextBox.Background = Brushes.Red;
            }
            else
            {
                RelationTextBox.Background = Brushes.White;
            }
        }

        private void CategoryTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CategoryTextBox.Text.Length > 100)
            {
                CategoryTextBox.ToolTip = "De tekst mag maximaal 100 tekens lang zijn";
                CategoryTextBox.Background = Brushes.Red;
            }
            else
            {
                CategoryTextBox.Background = Brushes.White;
            }
        }

    }
}
