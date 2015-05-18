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
    /// Interaction logic for ReportGUI.xaml
    /// </summary>
    public partial class ReportGUI : Window
    {
        private List<ReportGrid> repGrid = new List<ReportGrid>();

        public ReportGUI(User user, Form form)
        {
            InitializeComponent();
            ReportIDLabel.Content = form.Id;
            if (form.ClientList.Count < 1)
                MessageBox.Show("Error er zijn geen Clienten in dit rapport");
            if (form.ClientList.Count == 1)
            {
                Client1IDLabel.Content = form.ClientList[0].Id;
                function1Label.Content = form.ClientList[0].Function;
                Age1Label.Content = form.ClientList[0].Age;
            }
            if (form.ClientList.Count == 2)
            {
                Client1IDLabel.Content = form.ClientList[0].Id;
                function1Label.Content = form.ClientList[0].Function;
                Age1Label.Content = form.ClientList[0].Age;
                Client2IDLabel.Content = form.ClientList[1].Id;
                function1Label_Copy.Content = form.ClientList[1].Function;
                Age1Label_Copy.Content = form.ClientList[1].Age;
            }
            memoLabel.Content = form.Memo;
            catLabel.Content = form.Category;
            relLabel.Content = form.Relation;

            SendReportRequest(user, form);




        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var overview = new rapportenoverzicht();
            //overview.Show();
            this.Close();
        }

        private void SendReportRequest(User usr, Form fm)
        {
            Report result = new Report();
            USFO usfo = new USFO(usr, fm);

            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetReport"].ConnectionString, usfo);
            result = response.Content.ReadAsAsync<Report>().Result;

            LoadReport(result);

        }

        private void LoadReport(Report rep)
        {

            int j = 0;
            
            int count = 0;
            foreach (Question q in rep.QuestionList)
            {
                repGrid[j].Theme = q.Text;
                repGrid[j].Id = q.Id;
                j++;
            }
            for (int i = 0; i < rep.AnswerList[0].Count; i++)
            {
                repGrid[i].Help1 = rep.AnswerList[0][repGrid[i].Id].Help;
                repGrid[i].Score1 = rep.AnswerList[0][repGrid[i].Id].Score;
                repGrid[i].Valid = true;
            }
            for (int i = 0; i < rep.AnswerList[1].Count; i++)
            {
                repGrid[i].Help2 = rep.AnswerList[1][repGrid[i].Id].Help;
                repGrid[i].Score2 = rep.AnswerList[1][repGrid[i].Id].Score;
                count++;
                repGrid[i].Valid = true;
            }

            int k = 0;
            foreach (ReportGrid valid in repGrid)
            {
                bool check = valid.Valid;
                if(!check)
                    repGrid.RemoveAt(k);
                k++;
                       
            }


            
            
           

            foreach (ReportGrid report in repGrid)
                ReportListView.Items.Add(rep);




        }

       
    }
}
