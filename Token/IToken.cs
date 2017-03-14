namespace SimpleCalculatorGroup2.Token
{
    enum TokenType
    {
        NumberToken,
        OperationToken,
    }

    interface IToken
    {
        string StringValue { get;}
        TokenType Type { get;}
    }
}
