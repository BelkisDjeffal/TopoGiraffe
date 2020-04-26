﻿using System.Windows;
using TopoGiraffe.Noyau;
using TopoGiraffe;

namespace TopoGiraffe.BoitesDialogue
{
    /// <summary>
    /// Logique d'interaction pour DataDialog.xaml
    /// </summary>
    public partial class DataDialog : Window
    {
        public DataDialog()
        {
            InitializeComponent();
        }
        public string Equidistance
        {
            get { return EquidistanceTextBox.Text; }
            set { EquidistanceTextBox.Text = value; }
        }
        public string Max
        {
            get { return MaxTextBox.Text; }
            set { MaxTextBox.Text = value; }
        }
        public string Min
        {
            get { return MinTextBox.Text; }
            set { MinTextBox.Text = value; }
        }


        public string EchelleOnCanvas
        {
            get { return EchelleTextBoxOnCanvas.Text; }
            set { EchelleTextBoxOnCanvas.Text = value; }
        }

        public string EchelleOnField
        {
            get { return EchelleTextBoxOnField.Text; }
            set { EchelleTextBoxOnField.Text = value; }
        }




        private void OnOKClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
