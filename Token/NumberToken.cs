namespace SimpleCalculatorGroup2.Token
{
    class NumberToken: IToken
    {
        public NumberToken(double value)
        {
            Value = value;
        }

        public string StringValue
        {
            get { return Value.ToString(); }
        }

        public TokenType Type
        {
            get
            {
                return TokenType.NumberToken;
            }
        }

        public static NumberToken operator +(NumberToken val1, NumberToken val2)
        {
            return new NumberToken(val1.Value + val2.Value);
        }

        public static NumberToken operator -(NumberToken val1, NumberToken val2)
        {
            return new NumberToken(val1.Value - val2.Value);
        }

        public static NumberToken operator *(NumberToken val1, NumberToken val2)
        {
            return new NumberToken(val1.Value * val2.Value);
        }

        public static NumberToken operator /(NumberToken val1, NumberToken val2)
        {
            return new NumberToken(val1.Value / val2.Value);
        }

        public double Value { get; private set; }
    }
}
