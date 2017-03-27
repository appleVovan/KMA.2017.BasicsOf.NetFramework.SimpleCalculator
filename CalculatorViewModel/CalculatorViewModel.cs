using System.ComponentModel;
using System.Runtime.CompilerServices;
using Learning.Calculator.Models;
using Learning.Calculator.ViewModels.Properties;

namespace Learning.Calculator.ViewModels
{
    public class CalculatorViewModel:INotifyPropertyChanged
    {
        private CalculatorModel _calculatorModel;
        private string _visibleValue;

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

        public CalculatorViewModel()
        {
            _calculatorModel = new CalculatorModel();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
