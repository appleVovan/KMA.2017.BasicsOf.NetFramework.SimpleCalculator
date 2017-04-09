using System;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

namespace Learning.Calculator.Models.Token
{
    [DataContract]
    [Serializable]
    public class OperationToken : Token
    {
        #region Properties

        [DataMember]
        public Operation Value { get; private set; }

        #region Override

        public override TokenType Type
        {
            get { return TokenType.OperationToken; }
        }

        protected internal override string StringValue
        {
            get { return Value.Name(); }
        }

        #endregion

        #endregion

        #region Construcotrs

        internal OperationToken(Operation value, Guid modelGuid, int index) : base(modelGuid, index)
        {
            Value = value;
        }

        private OperationToken()
        {
        }

        #endregion

        #region EntityFrameworkConfiguration

        public class OperationTokenConfiguration : EntityTypeConfiguration<OperationToken>
        {
            public OperationTokenConfiguration()
            {
                ToTable("OperationToken");
                HasKey(t => t.Guid);
                Property(t => t.Value).HasColumnName("Value").IsRequired();
            }
        }

        #endregion
    }
}
