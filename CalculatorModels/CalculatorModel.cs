using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Learning.Calculator.Models.Token;

namespace Learning.Calculator.Models
{
    public class CalculatorModel
    {
        private readonly List<IToken> _tokens = new List<IToken>();
        private double _currentValue;
        private String _currentStringValue = "0";

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

        private void AddDigit(string content)
        {
            var symbol = content[0];

            if (!Char.IsDigit(symbol) && (symbol != '.' || (symbol == '.' && _currentStringValue.Contains('.'))))
                return;

            string tempValue = _currentStringValue + symbol;
            double tempNumber = 0;

            if (
                !Double.TryParse(tempValue, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"), out tempNumber))
                return;

            _currentValue = tempNumber;

            _currentStringValue = tempValue;

        }

        private void AddSymbol(string content)
        {
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

                if (tempOperation != Operation.LeftParenthesis &&
                    (lastOperation == null || lastOperation.Value != Operation.RightParenthesis))
                    _tokens.Add(new NumberToken(_currentValue));
                _tokens.Add(new OperationToken(tempOperation));
                if (tempOperation != Operation.LeftParenthesis)
                {
                    _currentValue = 0;
                    _currentStringValue = "0";
                }
            }

        }

        private void Calculate()
        {
            var operationStack = new Stack<OperationToken>();
            var outputQueue = new Queue<IToken>();

            foreach (var token in _tokens)
            {
                if (token.Type == TokenType.NumberToken)
                {
                    outputQueue.Enqueue(token);
                    continue;
                }
                var operationToken = token as OperationToken;
                if (operationToken == null)
                    throw new Exception();
                OperationToken lastOperation;
                switch (operationToken.Value)
                {
                    case Operation.Plus:
                    case Operation.Minus:
                        if (operationStack.Any())
                        {
                            lastOperation = operationStack.Peek();
                            if (lastOperation.Value != Operation.LeftParenthesis)
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

            var numberStack = new Stack<NumberToken>();

            while (outputQueue.Any())
            {
                IToken currentToken = outputQueue.Dequeue();
                if (currentToken.Type == TokenType.NumberToken)
                {
                    numberStack.Push(currentToken as NumberToken);
                    continue;
                }
                var currentOperation = currentToken as OperationToken;
                if (currentOperation == null)
                    throw new Exception();
                if (numberStack.Count < 2)
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
                        numberStack.Push(val1*val2);
                        break;
                    case Operation.Divide:
                        if (val2.Value == 0)
                        {
                            throw new Exception(Properties.Resources.Exception_DivideByZero);
                        }
                        numberStack.Push(val1/val2);
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
        }

        private void ClearData()
        {
            _tokens.Clear();
            _currentValue = 0;
            _currentStringValue = "0";
        }
    }
}
