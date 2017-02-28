using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public enum Operation
    {
        Unknown = 0,
        Plus,
        Minus,
        Multiply,
        Divide,
    }

    public static class Extensions
    {
        public static string GetName (this Operation enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>().Name;
        }

        public static TAttribute GetAttribute<TAttribute>(this Operation enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static Operation GetValueFromName(string name)
        {
            var type = typeof(Operation);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == name)
                    {
                        return (Operation)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == name)
                        return (Operation)field.GetValue(null);
                }
            }
            return Operation.Unknown;
        }
    }
}
