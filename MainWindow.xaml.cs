using System;
using System.Collections.Generic;
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

namespace SimpleCalculatorGroup2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string lastStringValue = "";
        private double rightValue, leftValue;
        private Operation operation;
        public MainWindow()
        {
            InitializeComponent();
            
           
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var pressedButton = sender as Button;
            if (pressedButton == null) 
                return;
            var content = (string) pressedButton.Content;
            double temp = 0;
            if (!Double.TryParse(lastStringValue + content, out temp) || temp>0)
            {
                return;
            }
            lastStringValue += content;
            leftValue = temp;
        }
    }

    enum Operation
    {
        Unknown = 0,
        Plus,
        Minus,
        Multiply,
        Divide,
    }
}
