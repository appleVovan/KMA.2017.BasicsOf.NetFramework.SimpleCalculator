using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using Learning.Calculator.Models;
using Learning.Calculator.Models.Token;
using Learning.Calculator.ViewModels.Properties;

namespace Learning.Calculator.ViewModels
{
    public class CalculatorViewModel:INotifyPropertyChanged
    {
        #region Fields
        private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Learning", "WPFCalculator");
        private CalculatorModel _calculatorModel;
        private List<CalculatorModel> _operations;
        private string _visibleValue;
        private RelayCommand _addDigitCommand;
        private RelayCommand _addSymbolCommand;
        private RelayCommand _clearCommand;
        private RelayCommand _equalCommand;
        private RelayCommand _refresh; 
        #endregion

        #region Properties
        public string VisibleValue
        {
            get { return _visibleValue; }
            set
            {
                if (_visibleValue != null && value == _visibleValue)
                    return;
                _visibleValue = value;
                OnPropertyChanged();
            }
        }

        public List<CalculatorModel> Operations
        {
            get { return _operations; }
            set
            {
                _operations = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand EqualCommand
        {
            get
            {
                return _equalCommand ?? (_equalCommand = new RelayCommand(obj =>Calculate()));
            }
        }
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refresh ?? (_refresh = new RelayCommand(obj => RefreshHistory()));
            }
        }
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(obj =>
                {
                    _calculatorModel.ClearData();
                    VisibleValue = _calculatorModel.ToString();
                    _calculatorModel.Serialize(_path);
                }));
            }
        }
        public RelayCommand AddSymbolCommand
        {
            get
            {
                return _addSymbolCommand ?? (_addSymbolCommand = new RelayCommand(AddSymbol));
            }
        }
        public RelayCommand AddDigitCommand
        {
            get
            {
                return _addDigitCommand ?? (_addDigitCommand = new RelayCommand(AddDigit));
            }
        } 
        #endregion

        #region Constructor
        public CalculatorViewModel()
        {
            _calculatorModel = CalculatorModel.Deserialize(_path);
            VisibleValue = _calculatorModel.ToString();
            RefreshHistory();
        } 
        #endregion

        #region Methods
        private void AddDigit(object obj)
        {
            string content;
            if (CheckValue(obj, out content))
            {
                _calculatorModel.AddDigit(content);
            }
            VisibleValue = _calculatorModel.ToString();
            _calculatorModel.Serialize(_path);
        }

        private void AddSymbol(object obj)
        {
            string content;
            if (CheckValue(obj, out content))
            {
                _calculatorModel.AddSymbol(content);
            }
            VisibleValue = _calculatorModel.ToString();
            _calculatorModel.Serialize(_path);
        }

        private bool CheckValue(object sender, out string content)
        {
            content = sender as string;
            return !String.IsNullOrWhiteSpace(content) && content.Length == 1;
        }

        private void RefreshHistory()
        {
            try
            {
                using (var myChannelFactory = new ChannelFactory<ICalculatorService>("CalculatorService"))
                {
                    ICalculatorService client = myChannelFactory.CreateChannel();
                    Operations = client.GetHistory();
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex, Strings.Exception_InCalculate);
                MessageBox.Show(String.Format(Strings.Exception_Default, Environment.NewLine, ex.Message));
            }
        }

        private void Calculate()
        {
            try
            {
                double result = 0;
                var lastOperation = _calculatorModel.Tokens.LastOrDefault() as OperationToken;
                if (lastOperation == null || lastOperation.Value == Operation.LeftParenthesis)
                    throw new Exception(Strings.Exception_LeftParenOrNumberExpected);

                if (lastOperation.Value != Operation.RightParenthesis)
                    _calculatorModel.Tokens.Add(new NumberToken(_calculatorModel.CurrentValue, _calculatorModel.Id, _calculatorModel.Index++));
                using (var myChannelFactory = new ChannelFactory<ICalculatorService>("CalculatorService"))
                {
                    ICalculatorService client = myChannelFactory.CreateChannel();
                    result = client.Calculate(_calculatorModel);
                }
                _calculatorModel = new CalculatorModel();
                _calculatorModel.UpdateValue(result);
                VisibleValue = _calculatorModel.ToString();
                _calculatorModel.Serialize(_path);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex, Strings.Exception_InCalculate);
                MessageBox.Show(String.Format(Strings.Exception_Default, Environment.NewLine, ex.Message));
            }
        } 
        #endregion

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
