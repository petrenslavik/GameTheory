using GamesTheory.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GamesTheory
{
    public class InputData : INotifyPropertyChanged
    {
        private int _strategyFirstPlayer;
        private int _strategySecondPlayer;
        private int _maxValue;
        private int _minValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public int StrategyFirstPlayer
        {
            get => _strategyFirstPlayer;
            set
            {
                if (value > 0)
                {
                    _strategyFirstPlayer = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                OnPropertyChanged();
            }
        }

        public int MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                OnPropertyChanged();
            }
        }

        public int StrategySecondPlayer
        {
            get => _strategySecondPlayer;
            set
            {
                if (value > 0)
                {
                    _strategySecondPlayer = value;
                    OnPropertyChanged();
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (IsCorrect())
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsCorrect()
        {
            if (_strategyFirstPlayer <= 0)
            {
                return false;
            }

            if (_strategySecondPlayer <= 0)
            {
                return false;
            }
            
            return true;
        }
    }
}
