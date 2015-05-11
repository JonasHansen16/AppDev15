using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for startpagina.xaml
    /// </summary>
    public partial class startpagina : Window
    {
        public startpagina()
        {
            InitializeComponent();
        }
        
        private void RapportAanvraagButton_Click(object sender, RoutedEventArgs e)
        {
            var winPatient = new aanmakenPatient();
            winPatient.Show();
            this.Close();
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("tot ziens ");
            var winLogin = new login();
            winLogin.Show();
            this.Close();
        }

        private void VragenlijstButton_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            try
            {
                SendRequest(client);
            }
            catch (Exception ex)
            {
                ConnectionFailure();
            }
            var winVraag = new vraag();
            winVraag.Show();
            this.Close();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            var winAdmin = new adminGui();
            winAdmin.Show();
            this.Close();
        }

        private void RapportenButton_Click(object sender, RoutedEventArgs e)
        {
            var winRapporten = new rapportenoverzicht();
            winRapporten.Show();
            this.Close();
        }

        private void SendRequest(Client client)
        {
            QuestionList result = new QuestionList();
            //get all Questions
            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["AllQuestions"].ConnectionString, client);
            result.Questions = response.Content.ReadAsAsync<List<Question>>().Result;

            if (result.Questions.Equals(null))
                QuestionsFailure();
            else
                WriteLocal(result);


        }

        private void QuestionsFailure()
        {
            MessageBox.Show("Geen vragen beschikbaar");
        }

        //This function writes a questionlist to local device
        private void WriteLocal(QuestionList allQuestions)
        {
            
            
            StreamWriter userWriter = new StreamWriter("/../../Questions/Questionnaire.txt", true);
            for (int i = 0; i < allQuestions.Questions.Count; i++ )
            {
                userWriter.WriteLine(allQuestions.Questions[i].Id);
                userWriter.WriteLine(allQuestions.Questions[i].Text);
                userWriter.WriteLine(allQuestions.Questions[i].Title);
            }
            userWriter.Close();
            
        }

        private void ConnectionFailure()
        {
            MessageBox.Show("Geen connectie met de database");
        }

        
    }
}
