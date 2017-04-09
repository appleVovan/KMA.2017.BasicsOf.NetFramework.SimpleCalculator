using System;
using System.Collections.Generic;
using System.Linq;
using Learning.Calculator.Functions.Properties;
using Learning.Calculator.Models;
using Learning.Calculator.Models.Token;

namespace Learning.Calculator.Functions
{
    public static class CalculatorFunctions
    {
        public static double Calculate(CalculatorModel model)
        {
            double result;
            try
            {
                Queue<Token> outputQueue;
                ParseTokens(model, out outputQueue);
                result = CalculateResult(outputQueue);
                model.ResultValue = result.ToString();
            }
            catch (Exception ex)
            {
                model.ResultValue = ex.Message;
                Logger.Logger.Log(ex, Strings.Exception_InCalculate);
                throw;
            }
            finally
            {
                EntityWrapper.EntityWrapper.SaveModel(model);
            }
            model.UpdateValue(result);
            return result;
        }

        private static void ParseTokens(CalculatorModel model, out Queue<Token> outputQueue)
        {
            var operationStack = new Stack<OperationToken>();
            outputQueue = new Queue<Token>();
            foreach (var token in model.Tokens)
            {
                if (token.Type == TokenType.NumberToken)
                {
                    outputQueue.Enqueue(token);
                    continue;
                }
                var operationToken = token as OperationToken;
                if (operationToken == null)
                    throw new Exception(String.Format(Strings.Exception_WrongObject, token.GetType().Name, typeof(OperationToken).Name));
                OperationToken lastOperation = null;
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
                            throw new Exception(Strings.Exception_LeftParenthesisExpected);
                        break;
                    default:
                        throw new Exception(Strings.Exception_WrongOperationType);
                }
            }
            while (operationStack.Any())
            {
                outputQueue.Enqueue(operationStack.Pop());
            }
        }

        private static double CalculateResult(Queue<Token> outputQueue)
        {
            var numberStack = new Stack<NumberToken>();

            while (outputQueue.Any())
            {
                Token currentToken = outputQueue.Dequeue();
                if (currentToken.Type == TokenType.NumberToken)
                {
                    numberStack.Push(currentToken as NumberToken);
                    continue;
                }
                var currentOperation = currentToken as OperationToken;
                if (currentOperation == null)
                    throw new Exception(String.Format(Strings.Exception_WrongObject, currentToken.GetType().Name, typeof(OperationToken).Name));
                if (numberStack.Count < 2)
                    throw new Exception(Strings.Exception_WrongAmountOfNumber);
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
                            throw new Exception(Strings.Exception_DivideByZero);
                        }
                        numberStack.Push(val1/val2);
                        break;
                }
            }

            if (numberStack.Count != 1)
            {
                throw new Exception(Strings.Exception_WrongAmountOfTokens);
            }
            return numberStack.Pop().Value;
        }
    }
}
