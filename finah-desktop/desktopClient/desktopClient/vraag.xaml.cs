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
        private int clientId = 1;  //clientId meegeven via parent
       
        private Button[] buttons;
        private Answer answer;
        private Client client;
        private QuestionList allQuestions;
        private List<Answer> _allAnswers;
        

        public vraag()
        {
            InitializeComponent();
            buttons = new Button[] { answer1Button, answer2Button, answer3Button, answer4Button, answer5Button, yesButton, noButton };

            allQuestions = new QuestionList();           
            answer = new Answer();
            client = new Client();
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
                

            if (b.Background != Brushes.Purple)
                changeAnswersToBeginState(b);

            b.Background = Brushes.Purple;

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
                buttons[currentAnswer - 1].Background = Brushes.Purple;
            }
            else
                helpStackPanel.Visibility = Visibility.Hidden;

            if (currentHelpAnswer == false)
            {
                yesButton.Background = Brushes.Purple;
                helpStackPanel.Visibility = Visibility.Visible;
            }
            else if (currentHelpAnswer == true)
            {
                noButton.Background = Brushes.Purple;
                helpStackPanel.Visibility = Visibility.Visible;
            }

            //ophalen van vraaginhoud
            byte[] byteImage;
            questionNumberTextBlock.Content = "Vraag " + allQuestions.Questions[currentQuestion].Id + allQuestions.Questions.Count;
            questionTextBlock.Text = allQuestions.Questions[currentQuestion].Text;
            HttpResponseMessage response =  ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["GetImage"].ConnectionString, allQuestions.Questions[currentQuestion].Id );
            byteImage = response.Content.ReadAsAsync<byte[]>().Result;
            

        }

        //bij een volgende, niet ingevulde vraag alle buttons terug normaal maken
        private void changeAnswersToBeginState(object sender)
        {
            
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(56, 63, 228));

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
                    StreamReader reader = new StreamReader("../../users/1.txt", true);
                    while (!reader.Peek().Equals(""))
                    {

                        currentAnswer.QuestionId = Convert.ToInt32(reader.ReadLine());
                        currentAnswer.Score = Convert.ToInt32(reader.ReadLine());
                        currentAnswer.Help = Convert.ToBoolean(reader.ReadLine());
                        finalAllAnswers.Add(currentAnswer);
                    }
                    HttpResponseMessage response = ApiConnection.genericRequest(System.Configuration.ConfigurationManager.ConnectionStrings["SendAnswers"].ConnectionString, finalAllAnswers);
                    result = response.Content.ReadAsAsync<bool>().Result;

                    if (result == false)
                    {

                    }
                    else
                    {

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

        /*public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
       } */
    }


}