using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Learning.Calculator.Models;
using Learning.Calculator.Models.Token;

namespace Learning.Calculator.WPFCalculator
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

        private List<IToken> _tokens = new List<IToken>();
        private double _currentValue = 0;
        private String _currentStringValue = "0";
        private double _rightNumber = 0;
        private double _leftNumber = 0;
        Operation _operation = 0;

        public bool IsPrinted
        {
            get
            {
                var lastToken = _tokens.LastOrDefault() as OperationToken;
                if (lastToken == null)
                    return true;
                return lastToken.Value != Operation.RightParenthesis;
            }
        }

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

                _currentValue = tempNumber;

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
                    var lastOperation = _tokens.LastOrDefault() as OperationToken;
                    if (lastOperation == null || lastOperation.Value == Operation.LeftParenthesis)
                        throw new Exception();

                    if (lastOperation.Value != Operation.RightParenthesis)
                        _tokens.Add(new NumberToken(_currentValue));

                        
                    Calculate();
                }
                else
                {
                    var lastOperation = _tokens.LastOrDefault() as OperationToken;

                    if(tempOperation != Operation.LeftParenthesis && (lastOperation == null || lastOperation.Value != Operation.RightParenthesis) )
                        _tokens.Add(new NumberToken(_currentValue));
                    _tokens.Add(new OperationToken(tempOperation));
                    if (tempOperation != Operation.LeftParenthesis)
                    {
                        _currentValue = 0;
                        _currentStringValue = "0";
                    }
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
            Stack<OperationToken> operationStack = new Stack<OperationToken>();
            Queue<IToken> outputQueue = new Queue<IToken>();

            foreach (var token in _tokens)
            {
                if (token.Type == TokenType.NumberToken)
                {
                    outputQueue.Enqueue(token);
                    continue;
                }
                OperationToken operationToken = token as OperationToken;
                if(operationToken == null)
                    throw new Exception();
                OperationToken lastOperation;
                switch (operationToken.Value)
                {
                    case Operation.Plus:
                    case Operation.Minus:
                        if (operationStack.Any())
                        {
                            lastOperation = operationStack.Peek();
                            if(lastOperation.Value != Operation.LeftParenthesis)
                                outputQueue.Enqueue(operationStack.Pop());
                        }
                            operationStack.Push(operationToken);
                        break;
                    case Operation.Multiply:
                    case Operation.Divide:
                        if (operationStack.Any())
                        {
                            lastOperation = operationStack.Peek();
                            if (lastOperation.Value == Operation.Multiply || lastOperation.Value == Operation.Divide)
                            outputQueue.Enqueue(operationStack.Pop());
                        }
                        operationStack.Push(operationToken);
                        break;
                    case Operation.LeftParenthesis:
                        operationStack.Push(operationToken);
                        break;
                    case Operation.RightParenthesis:
                        lastOperation = null;
                        while (operationStack.Any())
                        {
                            lastOperation = operationStack.Pop();
                            if (lastOperation.Value == Operation.LeftParenthesis)
                                break;
                            outputQueue.Enqueue(lastOperation);
                        }
                        if (lastOperation == null || lastOperation.Value != Operation.LeftParenthesis)
                            throw new Exception();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                
            }
            while (operationStack.Any())
            {
                outputQueue.Enqueue(operationStack.Pop());
            }

            Stack<NumberToken> numberStack = new Stack<NumberToken>();

            while (outputQueue.Any())
            {
                IToken currentToken = outputQueue.Dequeue();
                if (currentToken.Type == TokenType.NumberToken)
                {
                    numberStack.Push(currentToken as NumberToken);
                    continue;
                }
                var currentOperation = currentToken as OperationToken;
                if(currentOperation == null)
                    throw new Exception();
                if(numberStack.Count < 2)
                    throw new Exception();
                var val2 = numberStack.Pop();
                var val1 = numberStack.Pop();
                switch (currentOperation.Value)
                {
                    case Operation.Plus:
                        numberStack.Push(val1 + val2);
                        break;
                    case Operation.Minus:
                        numberStack.Push(val1 - val2);
                        break;
                    case Operation.Multiply:
                        numberStack.Push(val1 * val2);
                        break;
                    case Operation.Divide:
                        if (val2.Value == 0)
                        {
                            throw new Exception(Properties.Resources.Exception_DivideByZero);
                        }
                        numberStack.Push(val1 / val2);
                        break;
                }
            }

            if (numberStack.Count != 1)
            {
                throw new Exception();
            }
            
            ClearData();
            _currentValue = numberStack.Pop().Value;
            _currentStringValue = _currentValue.ToString();
            PrintValue();
        }
        private void PrintValue()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var token in _tokens)
            {
                builder.AppendFormat("{0} ", token.StringValue);
            }
            if (IsPrinted)
                builder.Append(_currentValue.ToString());
            Box.Text = builder.ToString();

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
            ClearData();
            PrintValue();
        }

        private void ClearData()
        {
            _tokens.Clear();
            _currentValue = 0;
            _currentStringValue = "0";
        }
    }

   
}
