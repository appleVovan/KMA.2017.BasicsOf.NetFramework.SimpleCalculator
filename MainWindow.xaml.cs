using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            Thread.CurrentThread.CurrentCulture=new CultureInfo("ru-Ru");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-Ru");

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pressedButton = sender as Button;
                if (pressedButton == null)
                    return;
                var content = (string)pressedButton.Content;
                if (String.IsNullOrWhiteSpace(content) || content.Length != 1)
                    return;
                char symbol = content[0];
                if (Char.IsDigit(symbol))
                {
                    String tempValue = lastStringValue + symbol;
                    double tempNumber = 0;
                    if (Double.TryParse(tempValue, out tempNumber))
                    {
                        lastStringValue += symbol;
                    }
                    else return;

                    if (operation == Operation.Unknown)
                    {
                        leftValue = tempNumber;
                    }
                    else rightValue = tempNumber;
                }
                else
                {
                    Operation tempOperation = Extensions.GetValueFromName(content);
                    if (tempOperation == Operation.Unknown)
                        return;
                    if (tempOperation == Operation.Equal)
                    {
                        if (operation == Operation.Unknown)
                            return;
                        switch (operation)
                        {
                            case Operation.Plus:
                                leftValue += rightValue;
                                break;
                            case Operation.Minus:
                                leftValue -= rightValue;
                                break;
                            case Operation.Multiply:
                                leftValue *= rightValue;
                                break;
                            case Operation.Divide:
                                if (rightValue == 0) throw new Exception(Properties.Resources.Exception_ZeroDivision);

                                leftValue /= rightValue;
                                break;
                            default:
                                return;
                        }
                        rightValue = 0;
                        operation = Operation.Unknown;
                    }
                    else
                    {
                        operation = tempOperation;
                        lastStringValue = "";
                        
                    }
                }
                Box.Text = operation == Operation.Unknown ? leftValue.ToString() : leftValue + operation.GetName() + rightValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            leftValue = rightValue = 0;
            lastStringValue = "";
            operation = Operation.Unknown;
            Box.Text = "";
        }
    }

    public enum Operation
    {
        Unknown = 0,
        [Display(Name = "+")]
        Plus,
        [Display(Name = "-")]
        Minus,
        [Display(Name = "*")]
        Multiply,
        [Display(Name = "/")]
        Divide,
        [Display(Name = "=")]
        Equal,
    }

    public static class Extensions
    {
        public static string GetName (this Operation enumValue)
        {
            return enumValue.GetAttribute<DisplayAttribute>().Name;
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
