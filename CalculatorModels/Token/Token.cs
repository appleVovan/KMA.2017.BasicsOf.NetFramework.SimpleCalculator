using System;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

namespace Learning.Calculator.Models.Token
{
    [DataContract]
    [Serializable]
    public abstract class Token
    {
        #region Properties

        [DataMember]
        protected internal Guid Guid { get; private set; }

        [DataMember]
        internal Guid ModelGuid { get; private set; }

        [DataMember]
        public int Index { get; private set; }

        #region AbstractProperties

        protected internal abstract string StringValue { get; }
        public abstract TokenType Type { get; }

        #endregion

        #region Associations

        protected internal virtual CalculatorModel Parent { get; private set; }

        #endregion

        #endregion

        #region Constructors

        protected internal Token()
        {
        }

        protected internal Token(Guid modelGuid, int index)
        {
            Guid = Guid.NewGuid();
            ModelGuid = modelGuid;
            Index = index;
        }

        #endregion

        #region EntityFrameworkConfiguration

        public class TokenConfiguration : EntityTypeConfiguration<Token>
        {
            public TokenConfiguration()
            {
                ToTable("Token");
                HasKey(t => t.Guid);
                Property(t => t.Guid).HasColumnName("Guid").IsRequired();
                Property(t => t.ModelGuid).HasColumnName("ModelGuid").IsRequired();
                Property(t => t.Index).HasColumnName("Index").IsRequired();

                Ignore(t => t.StringValue);
                Ignore(t => t.Type);
            }
        }

        #endregion
    }
}
