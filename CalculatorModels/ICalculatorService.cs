using System.Collections.Generic;
using System.ServiceModel;
using Learning.Calculator.Models.Token;

namespace Learning.Calculator.Models
{
    [ServiceContract]
    public interface ICalculatorService
    {
        [OperationContract]
        [ServiceKnownType(typeof(NumberToken))]
        [ServiceKnownType(typeof(OperationToken))]
        double Calculate(CalculatorModel calculatorModel);
        [OperationContract]
        [ServiceKnownType(typeof(NumberToken))]
        [ServiceKnownType(typeof(OperationToken))]
        List<CalculatorModel> GetHistory();
    }
}
