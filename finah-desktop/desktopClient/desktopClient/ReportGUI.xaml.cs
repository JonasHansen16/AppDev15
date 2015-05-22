using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ReportGUI.xaml
    /// </summary>
    public partial class ReportGUI : Window
    {
        private List<ReportGrid> repGrid = new List<ReportGrid>();
        private User _user;
        private Report report = null;

        public ReportGUI(User user, Form form)
        {
            InitializeComponent();
            _user = user;
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
            Window window = new startpagina(_user);
            
            this.Close();
            window.Show();
        }

        private void SendReportRequest(User usr, Form fm)
        {
            Report result = new Report();
            USFO usfo = new USFO(usr, fm);
            try
            {
                
                HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetReport"].ConnectionString, usfo);
                result = response.Content.ReadAsAsync<Report>().Result;
                report = result;
                result.ClientList = new List<ClientExp>(result.ClientList);
                result.QuestionList = new List<Question>(result.QuestionList);
                List<List<Answer>> temp = new List<List<Answer>>();
                for (int i = 0; i < result.AnswerList.Count; i++)
                {
                    temp.Add(new List<Answer>(result.AnswerList[i]));
                }
                result.AnswerList = temp;

                    

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opgetreden tijdens connectie met de database " + ex);
            }
            LoadReport(result);
           

        }

        private void LoadReport(Report rep)
        {

            int j = 0;
            
            int count = 0;
            foreach (Question q in rep.QuestionList)
            {
                repGrid.Add(new ReportGrid());
                repGrid[j].Theme = q.Text;
                repGrid[j].Id = q.Id;
                j++;
            }
            for (int i = 0; i < rep.AnswerList[0].Count; i++)
            {
                repGrid[i].Help1 = rep.AnswerList[0][i].Help;
                repGrid[i].Score1 = rep.AnswerList[0][i].Score;
                repGrid[i].Valid = true;
            }
            for (int i = 0; i < rep.AnswerList[1].Count; i++)
            {
                repGrid[i].Help2 = rep.AnswerList[1][i].Help;
                repGrid[i].Score2 = rep.AnswerList[1][i].Score;
                count++;
                repGrid[i].Valid = true;
            }

            
            for (int i = 0; i < repGrid.Count; i++)
            {
                bool check = repGrid[i].Valid;
                if (!check)
                {
                    repGrid.RemoveAt(i);
                    i--;
                }
            }

          






            ReportDataGrid.ItemsSource = repGrid;
            




        }
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = propertyDescriptor.DisplayName;
            if (propertyDescriptor.DisplayName == "Valid")
            {
                e.Cancel = true;
            }
        }

        private void PdfButton_Click(object sender, RoutedEventArgs e)
        {
            if(report == null)
            {
                MessageBox.Show("Geen rapport beschikbaar");
                return;
            }
            string path;
            path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "nah");
            
           if(!Directory.Exists(path))
           {
               Directory.CreateDirectory(path);
           }
                
            path = System.IO.Path.Combine(path, ReportIDLabel.Content + ".pdf");
            PdfReportGenerator pdfgeneration = new PdfReportGenerator();
            try
            {
                pdfgeneration.reportToPdf(report, (int)ReportIDLabel.Content, path);
                MessageBox.Show("Rapport aangemaakt op plaats " + path);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Er is iets fout gegaan tijdens het maken van de PDF" + ex);
            }
           

        }

       
    }
}
