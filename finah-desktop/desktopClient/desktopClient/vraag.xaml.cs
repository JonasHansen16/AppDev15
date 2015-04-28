using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    /// Interaction logic for vraag.xaml
    /// </summary>
    public partial class vraag : Window
    {
        private int currentQuestion;
        private int clientId = 1;  //clientId meegeven via parent
        private int[] currentSessionInfo;
        private Button[] buttons;
        private Answer answer;
        

        public vraag()
        {
            InitializeComponent();
            buttons = new Button[] { answer1Button, answer2Button, answer3Button, answer4Button, answer5Button, yesButton, noButton };

            //array met answerwaarden, worden weggeschreven in vorm: clientId-currentQuestion-answer-helpanswer
            answer = new Answer();
            answer.ClientId = clientId;
            currentSessionInfo = new int[180];

            currentSessionInfo[0] = 23; //clientid

            currentQuestion = 1;

            loadQuestion();
        }
        //antwoorden paars maken, en antwoord in array zetten
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

            int placeArray = getPlaceArray(currentQuestion);

            //antwoorden in array zetten, 6 en 7 staan voor de antwoorden op hulpvraag
            if (selectedAnswer == 6)
            {
                answer.Help = 1;
                currentSessionInfo[placeArray + 1] = 1; //hulpvraag antwoorden staan een plek verder
            }
            else if (selectedAnswer == 7)
            {
                answer.Help = 2;
                currentSessionInfo[placeArray + 1] = 2;
            }
            else
                answer.AnswerButton = selectedAnswer;
                currentSessionInfo[placeArray] = selectedAnswer;

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
            
            int currentPlace = getPlaceArray(currentQuestion); //huidige plaats in de array
            int currentAnswer = currentSessionInfo[currentPlace];
            int currentHelpAnswer = currentSessionInfo[currentPlace + 1];

            changeAnswersToBeginState(null);

            if (currentAnswer != 0) //als er voor die vraag nog niks is ingevuld
            {
                buttons[currentAnswer - 1].Background = Brushes.Purple;
            }
            else
                helpStackPanel.Visibility = Visibility.Hidden;

            if (currentHelpAnswer == 1)
            {
                yesButton.Background = Brushes.Purple;
                helpStackPanel.Visibility = Visibility.Visible;
            }
            else if (currentHelpAnswer == 2)
            {
                noButton.Background = Brushes.Purple;
                helpStackPanel.Visibility = Visibility.Visible;
            }

            //ophalen van vraaginhoud
            questionNumberTextBlock.Content = "Vraag " + currentQuestion + "/45";
            questionTextBlock.Text = File.ReadLines("../../questions/questions.txt").Skip(currentQuestion - 1).Take(1).First();
            questionImage.Source = new BitmapImage(new Uri("../../ImagesQ/" + currentQuestion + ".jpg", UriKind.Relative));
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

        //achterhalen waar in de array het antwoord op de huidige vraag staat
        private int getPlaceArray(int currentQuestion)
        {
            int currentPlace = (currentQuestion - 1) * 4 + 2;

            return currentPlace;
        }

        /*bijhouden van de antwoorden wanneer de applicatie offline wordt gebruikt
         * later worden die in de database opgeslagen
         * ze staan in de vorm: clientId-currentQuestion-answer-helpAnswer
         */
        private void writeUserToTextFile()
        {
            StreamWriter userWriter = new StreamWriter("../../users/users.txt", true);

            foreach (int value in currentSessionInfo)
            {
                userWriter.Write(value);
            }
            userWriter.WriteLine();

            userWriter.Close();
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion != 1)
            {
                currentQuestion--;
                loadQuestion();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion != 45)
            {
                answer.writeAnswerToTextFile();
                answer = new Answer(clientId);
                currentQuestion++;
                loadQuestion();
            }
        }

        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuestion = 1;

            for (int i = 2; i < 180; i++)
            {
                currentSessionInfo[i] = 0;
            }

            loadQuestion();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion < 45)
            {
                currentQuestion++;
                writeUserToTextFile();
                answer = new Answer(clientId);
                loadQuestion();

                if (currentQuestion == 45)
                {
                    confirmButton.Content = "Bevestig vragenlijst";

                    
                }

            }
            else
            {
                questionGrid.Visibility = Visibility.Hidden;
                succesStackPanel.Visibility = Visibility.Visible;
            }

        }
    }


}