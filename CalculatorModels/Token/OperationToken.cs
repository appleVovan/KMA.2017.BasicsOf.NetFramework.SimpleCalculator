namespace Learning.Calculator.Models.Token
{
    public class OperationToken: IToken
    {
        public OperationToken(Operation value)
        {
            Value = value;
        }

        public string StringValue
        {
            get { return Value.Name(); }
        }

        public TokenType Type
        {
            get
            {
                return TokenType.OperationToken;
            }
        }

        public Operation Value { get; private set; }
    }
}
