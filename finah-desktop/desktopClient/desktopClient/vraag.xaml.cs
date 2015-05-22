using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace sprint_1_def
{
    /// <summary>
    /// Interaction logic for vraag.xaml
    /// </summary>
    public partial class vraag : Window
    {
        private int currentQuestion;
        
       
        private Button[] buttons;
        private Answer answer;
        private Client _client;
        private QuestionList allQuestions;
        private List<Answer> _allAnswers;
        

        public vraag(Client client)
        {
            InitializeComponent();
            SendRequest(client);
            buttons = new Button[] { answer1Button, answer2Button, answer3Button, answer4Button, answer5Button, yesButton, noButton };

            allQuestions = new QuestionList();           
            answer = new Answer();
            _client = client;
            
            GetQuestionList();
            _allAnswers = new List<Answer>();
            
           

            currentQuestion = 0;

            loadQuestion();
        }
        //antwoorden paars maken, en antwoord in answer object zetten
        private void selectAnswer(object sender, RoutedEventArgs e)
        {
            
            Button b = ((Button)e.Source);
            int selectedAnswer = 0;
            
            //achterhalen welk antwoord geselecteerd is
            for (int i = 0; i < 7; i++)
            {
                if (buttons[i] == b)
                {
                    selectedAnswer = i + 1;
                    i = 7;
                }
            }

            

            //antwoorden in answer zetten, 6 en 7 staan voor de antwoorden op hulpvraag
            if (selectedAnswer == 6)
            {
                answer.Help = false;
                
            }
            else if (selectedAnswer == 7)
            {
                answer.Help = true;
                
            }
            else
                answer.Score = selectedAnswer;


            if (b.Background != System.Windows.Media.Brushes.Purple)
                changeAnswersToBeginState(b);

            b.Background = System.Windows.Media.Brushes.Purple;

            //hulpvraag visible maken als als de laatste 3 antwoorden geselecteerd worden
            if (b == answer3Button || b == answer4Button || b == answer5Button || b == yesButton || b == noButton)
            {
                helpStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                
                helpStackPanel.Visibility = Visibility.Hidden;
            }
        }

        //het laden van een vraag
        private void loadQuestion()
        {
            
             //huidige plaats in de array
            int currentAnswer = _allAnswers[currentQuestion].Score;
            bool currentHelpAnswer = _allAnswers[currentQuestion].Help;
            

            changeAnswersToBeginState(null);

            if (currentAnswer != 0) //als er voor die vraag nog niks is ingevuld
            {
                buttons[currentAnswer - 1].Background = System.Windows.Media.Brushes.Purple;
            }
            else
                helpStackPanel.Visibility = Visibility.Hidden;

            if (currentHelpAnswer == false)
            {
                yesButton.Background = System.Windows.Media.Brushes.Purple;
                helpStackPanel.Visibility = Visibility.Visible;
            }
            else if (currentHelpAnswer == true)
            {
                noButton.Background = System.Windows.Media.Brushes.Purple;
                helpStackPanel.Visibility = Visibility.Visible;
            }

            //ophalen van vraaginhoud
            byte[] byteImage;
            questionNumberTextBlock.Content = "Vraag " + allQuestions.Questions[currentQuestion].Id + allQuestions.Questions.Count;
            questionTextBlock.Text = allQuestions.Questions[currentQuestion].Text;
            CLID clid = new CLID();
            clid.CL = _client;
            clid.ID = allQuestions.Questions[currentQuestion].Id;

            HttpResponseMessage response =  ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetImage"].ConnectionString, clid );
            byteImage = response.Content.ReadAsAsync<byte[]>().Result;
            questionImage.Source = ByteToImage(byteImage);
            

        }

        //bij een volgende, niet ingevulde vraag alle buttons terug normaal maken
        private void changeAnswersToBeginState(object sender)
        {

            SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(56, 63, 228));

            if (sender != yesButton && sender != noButton)
            {
                foreach (Button button in buttons)
                {
                    button.Background = brush;
                }
            }

            if (sender == yesButton)
                noButton.Background = brush;
            else if (sender == noButton)
                yesButton.Background = brush;
        }

    

        
        

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion != 0)
            {
                currentQuestion--;
                loadQuestion();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion != allQuestions.Questions.Count)
            {
                
                currentQuestion++;
                loadQuestion();
            }
        }

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuestion = 0;
            _allAnswers = new List<Answer>();
            loadQuestion();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            writeAnswerToTextFile();
            if (currentQuestion < allQuestions.Questions.Count-1)
            {
                _allAnswers.Add(answer);
                currentQuestion++;               
                answer = new Answer();
                loadQuestion();

                if (currentQuestion == allQuestions.Questions.Count-1)
                {
                    bool result;
                    List<Answer> finalAllAnswers = new List<Answer>();
                    Answer currentAnswer = new Answer();
                    StreamReader reader = new StreamReader("../../users/"+_client.Id+".txt", true);
                    while (!reader.Peek().Equals(""))
                    {

                        currentAnswer.QuestionId = Convert.ToInt32(reader.ReadLine());
                        currentAnswer.Score = Convert.ToInt32(reader.ReadLine());
                        currentAnswer.Help = Convert.ToBoolean(reader.ReadLine());
                        finalAllAnswers.Add(currentAnswer);
                    }
                    CLANL clanl = new CLANL();
                    clanl.ANL = finalAllAnswers;
                    clanl.CL = _client;
                    HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["SendAnswers"].ConnectionString, clanl);
                    result = response.Content.ReadAsAsync<bool>().Result;

                    if (result == false)
                    {
                        MessageBox.Show("Fout opgetreden tijdens het wegschrijven van de antwoorden naar de database");
                        var window = new login();
                        window.Show();
                        this.Close();
                    }
                    else
                    {
                        File.Delete("../../users/"+_client.Id+".txt");
                        HttpResponseMessage checkresponse = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["CheckAllAnswered"].ConnectionString, _client);
                        result = checkresponse.Content.ReadAsAsync<bool>().Result;
                        if (result == true)
                        {
                            var window = new login();
                            window.Show();
                            this.Close();
                        }
                        else
                        {
                            HttpResponseMessage checkresponse2 = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetAllUnanswered"].ConnectionString, _client);
                            allQuestions.Questions = checkresponse2.Content.ReadAsAsync<List<Question>>().Result;
                            currentQuestion = 0;
                            loadQuestion();
                            _allAnswers = new List<Answer>();
                            answer = new Answer();
                        }

                    }
                    confirmButton.Content = "Bevestig vragenlijst";


                    
                }

            }
            else
            {
                questionGrid.Visibility = Visibility.Hidden;
                succesStackPanel.Visibility = Visibility.Visible;
            }

        }

        private void writeAnswerToTextFile()
        {
            
            
            StreamWriter userWriter = new StreamWriter("../../users/1.txt", true);

            userWriter.WriteLine(answer.QuestionId);
            userWriter.WriteLine(answer.Score);
            userWriter.WriteLine(answer.Help);

            userWriter.Close();
        }

        private void GetQuestionList()
        {
            Question question = new Question();
            StreamReader reader = new StreamReader("../../questions/Questionnaire.txt", true);
            while (!reader.Peek().Equals(""))
            {
                
                question.Id = Convert.ToInt32(reader.ReadLine());
                question.Text = reader.ReadLine();
                question.Title = reader.ReadLine();
                allQuestions.Questions.Add(question);
            }
        }

        //converts byte array to image
        private ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        private void SendRequest(Client client)
        {
            QuestionList result = new QuestionList();
            //get all Questions
            HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["AllQuestions"].ConnectionString, client);
            result.Questions = response.Content.ReadAsAsync<List<Question>>().Result;

            if (result.Questions.Count == 0)
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
            for (int i = 0; i < allQuestions.Questions.Count; i++)
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