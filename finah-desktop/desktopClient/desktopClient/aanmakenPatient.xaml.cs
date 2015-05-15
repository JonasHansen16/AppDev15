﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        

        private void terugButton_Click(object sender, RoutedEventArgs e)
        {
            var winStart = new startpagina(user);
            winStart.Show();
            this.Close();
        }

        private void OverView_Click(object sender, RoutedEventArgs e)
        {
            var winRapporten = new rapportenoverzicht(user);
            winRapporten.Show();
            this.Close();
        }

        

        
    }
}
