using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Learning.Calculator.Models.Token;

namespace Learning.Calculator.Models
{
    [DataContract]
    [Serializable]
    public class CalculatorModel
    {
        #region Fields

        public int Index = 0;
        [DataMember] private Guid _id;
        [DataMember] private List<Token.Token> _tokens = new List<Token.Token>();
        [DataMember] private double _currentValue;
        [DataMember] private string _resultValue;
        [DataMember] private String _currentStringValue = "0";
        private string _tokenView;

        #endregion

        #region Properties

        public Guid Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public double CurrentValue
        {
            get { return _currentValue; }
            private set { _currentValue = value; }
        }

        private string CurrentStringValue
        {
            get { return _currentStringValue; }
            set { _currentStringValue = value; }
        }

        public string ResultValue
        {
            get { return _resultValue; }
            set { _resultValue = value; }
        }

        #region Dynamic

        public string TokenView
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_tokenView))
                {
                    _tokenView = ToString();
                }
                return _tokenView;
            }
        }

        private bool IsPrinted
        {
            get
            {
                var lastToken = _tokens.LastOrDefault() as OperationToken;
                if (lastToken == null)
                {
                    return (_tokens.LastOrDefault() as NumberToken) == null;
                }
                return lastToken.Value != Operation.RightParenthesis;
            }
        }

        #endregion

        #region Associations

        public virtual List<Token.Token> Tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }
        
        #endregion

        #endregion

        #region Constructors

        public CalculatorModel()
        {
            _id = Guid.NewGuid();
        }

        #endregion

        #region Methods
        
        public void UpdateValue(double value)
        {
            ClearData(value);
        }

        public void ClearData(double value = 0)
        {
            _tokens.Clear();
            _currentValue = value;
            _currentStringValue = value.ToString();
        }

        public void AddDigit(string content)
        {
            var symbol = content[0];

            if (!Char.IsDigit(symbol) && (symbol != '.' || (symbol == '.' && _currentStringValue.Contains('.'))))
                return;

            string tempValue = _currentStringValue + symbol;
            double tempNumber = 0;

            if (!Double.TryParse(tempValue, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"), out tempNumber))
                return;

            _currentValue = tempNumber;
            _currentStringValue = tempValue;
        }

        public void AddSymbol(string content)
        {
            Operation tempOperation = Extensions.GetValueFromName(content);
            if (tempOperation == Operation.Unknown || tempOperation == Operation.Equal)
                return;

            var lastOperation = _tokens.LastOrDefault() as OperationToken;

            if (tempOperation != Operation.LeftParenthesis &&
                (lastOperation == null || lastOperation.Value != Operation.RightParenthesis))
                _tokens.Add(new NumberToken(_currentValue, Id, Index++));
            _tokens.Add(new OperationToken(tempOperation, Id, Index++));
            if (tempOperation == Operation.LeftParenthesis) 
                return;
            _currentValue = 0;
            _currentStringValue = "0";
        }

        #endregion

        #region Override
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var token in _tokens)
            {
                builder.AppendFormat("{0} ", token.StringValue);
            }
            if (IsPrinted)
                builder.Append(_currentValue.ToString());
            return builder.ToString();
        }

        #endregion

        #region EntityFrameworkConfigurtion

        public class CalculatorModelConfiguration : EntityTypeConfiguration<CalculatorModel>
        {
            public CalculatorModelConfiguration()
            {
                ToTable("CalculatorModel");
                HasKey(t => t.Id);
                Property(t => t.ResultValue).HasColumnName("ResultValue").IsRequired();
                HasMany(t => t.Tokens)
                    .WithRequired(t => t.Parent)
                    .HasForeignKey(t => t.ModelGuid)
                    .WillCascadeOnDelete(true);

                Ignore(t => t.IsPrinted);
                Ignore(t => t.CurrentValue);
                Ignore(t => t.CurrentStringValue);
                Ignore(t => t.TokenView);
            }
        }

        #endregion

        #region Serialization

        public void Serialize(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(path, "LastModel.calcm");
                if (File.Exists(path))
                    File.Delete(path);
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                stream.Close();
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex, "In Serialize");
            }
        }

        public static CalculatorModel Deserialize(string path)
        {
            CalculatorModel obj = null;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(path, "LastModel.calcm");
                Stream stream = null;
                if (File.Exists(path))
                {
                    try
                    {
                        IFormatter formatter = new BinaryFormatter();
                        stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                        obj = (CalculatorModel) formatter.Deserialize(stream);
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }
            }
            catch
                (Exception ex)
            {
                Logger.Logger.Log(ex, "In Serialize");
            }
            return obj ?? new CalculatorModel();
        }

        #endregion
    }
}
