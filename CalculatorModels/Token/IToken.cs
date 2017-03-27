namespace Learning.Calculator.Models.Token
{
    public enum TokenType
    {
        NumberToken,
        OperationToken,
    }

    public interface IToken
    {
        string StringValue { get;}
        TokenType Type { get;}
    }
}
