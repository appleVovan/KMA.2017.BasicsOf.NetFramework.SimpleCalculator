using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SimpleCalculatorGroup2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("uk-UA");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("uk-UA");
            PrintValue();
        }

        private String _currentStringValue = "0";
        private double _rightNumber = 0;
        private double _leftNumber = 0;
        Operation _operation = 0;

        private void ButtonDigit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content;
                if (!CheckButton(sender, out content))
                    return;
                var symbol = content[0];
                
                if (!Char.IsDigit(symbol) && (symbol != '.' || (symbol == '.' && _currentStringValue.Contains('.'))))
                    return;

                string tempValue = _currentStringValue + symbol;
                double tempNumber = 0;

                if (!Double.TryParse(tempValue, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"),out tempNumber))
                    return;

                if (_operation == Operation.Unknown)
                    _leftNumber = tempNumber;
                else
                    _rightNumber = tempNumber;

                _currentStringValue = tempValue;

                PrintValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.Exception_Default, Environment.NewLine, ex.Message));
            }
        }
        private void ButtonSymbol_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content;
                if (!CheckButton(sender, out content))
                    return;
                Operation tempOperation = Extensions.GetValueFromName(content);
                if (tempOperation == Operation.Unknown)
                    return;

                if (tempOperation == Operation.Equal)
                {
                    if (_operation == Operation.Unknown)
                        return;
                    Calculate();
                }
                else
                {
                    if (_operation!=Operation.Unknown && _rightNumber!=0)
                        return;
                    _operation = tempOperation;
                    _currentStringValue = "0";
                }

                PrintValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.Exception_Default, Environment.NewLine, ex.Message));
            }
        }
        private void Calculate()
        {
            switch (_operation)
            {
                case Operation.Plus:
                    _leftNumber += _rightNumber;
                    break;
                case Operation.Minus:
                    _leftNumber -= _rightNumber;
                    break;
                case Operation.Multiply:
                    _leftNumber *= _rightNumber;
                    break;
                case Operation.Divide:
                    if (_rightNumber == 0)
                    {
                        throw new Exception(Properties.Resources.Exception_DivideByZero);
                    }
                    _leftNumber /= _rightNumber;
                    break;
            }
            _rightNumber = 0;
            _currentStringValue = _leftNumber.ToString();
            _operation = Operation.Unknown;
        }
        private void PrintValue()
        {
            Box.Text = _operation == Operation.Unknown
                    ? _leftNumber.ToString()
                    : _leftNumber + _operation.Name() + _rightNumber;
        }

        private bool CheckButton(object sender, out string content)
        {
            content = null;
            var pressedButton = sender as Button;
            if (pressedButton == null)
                return false;
            content = (string)pressedButton.Content;
            return !String.IsNullOrWhiteSpace(content) && content.Length == 1;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            _rightNumber = _leftNumber = 0;
            _operation = Operation.Unknown;
            Box.Clear();
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
        [Display(Name = "(")]
        LeftParenthesis,
        [Display(Name = ")")]
        RightParenthesis,
    }

    public static class Extensions
    {
        public static string Name(this Operation enumValue)
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
