
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TopoGiraffe.BoitesDialogue
{
   

    public partial class Window1 : Window
    {

        //colors related 
        class RectangleName
        {
            public Rectangle Rect { get; set; }
            public string Name { get; set; }
        }


        //Contructor
        public Window1()
        {
            InitializeComponent();

            styleCourbeCmbInDialogBox.SelectedIndex = 0;
        }


        //buttons behaviour
        void OkButton_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = true;
        }

        //number textbox control
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }



        
        string alti;
        private void Altitude_TextChanged(object sender, TextChangedEventArgs e)
        {
            alti = Altitude.Text;
        }
        //number textbox control

    }


}


