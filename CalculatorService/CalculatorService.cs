using System.Collections.Generic;
using Learning.Calculator.Functions;
using Learning.Calculator.Models;

namespace Learning.Calculator.Service
{
    public class CalculatorService:ICalculatorService
    {
        public double Calculate(CalculatorModel calculatorModel)
        {
            return CalculatorFunctions.Calculate(calculatorModel);
        }

        public List<CalculatorModel> GetHistory()
        {
            return EntityWrapper.EntityWrapper.GetModels();
        }
    }
}
