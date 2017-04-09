using System;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

namespace Learning.Calculator.Models.Token
{
    [DataContract]
    [Serializable]
    public class NumberToken : Token
    {
        #region Properties

        [DataMember]
        public double Value { get; private set; }

        #region Override

        protected internal override string StringValue
        {
            get { return Value.ToString(); }
        }

        public override TokenType Type
        {
            get { return TokenType.NumberToken; }
        }

        #endregion

        #endregion

        #region Constructor

        public NumberToken(double value, Guid modelGuid, int index) : base(modelGuid, index)
        {
            Value = value;
        }

        private NumberToken(double value)
        {
            Value = value;
        }

        private NumberToken()
        {
        }

        #endregion

        #region Operators

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
            return new NumberToken(val1.Value*val2.Value);
        }

        public static NumberToken operator /(NumberToken val1, NumberToken val2)
        {
            return new NumberToken(val1.Value/val2.Value);
        }

        #endregion

        #region EntityFrameworkConfiguration

        public class NumberTokenConfiguration : EntityTypeConfiguration<NumberToken>
        {
            public NumberTokenConfiguration()
            {
                ToTable("NumberToken");

                HasKey(t => t.Guid);
                Property(t => t.Value).HasColumnName("Value").IsRequired();

            }
        }

        #endregion
    }
}
