﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TopoGiraffe.Boites_de_dialogue
{
    /// <summary>
    /// Interaction logic for Pente.xaml
    /// </summary>
    public partial class Pente : Window
    {
        public Pente(String pente)
        {
            InitializeComponent();
            penteb.Text = pente;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}