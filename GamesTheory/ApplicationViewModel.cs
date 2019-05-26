using GamesTheory.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace GamesTheory
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private PaymentMatrix _paymentMatrix;
        private bool _isGenerated;
        private DataGrid _matrix;
        private DataGrid _priceMatrix;
        public event PropertyChangedEventHandler PropertyChanged;
        public PaymentMatrix PaymentMatrix
        {
            get => _paymentMatrix;
            set { _paymentMatrix = value; OnPropertyChanged(); }
        }

        public InputData InputData { get; set; }

        public bool IsGenerated
        {
            get => _isGenerated;
            set { _isGenerated = value; OnPropertyChanged(); }
        }

        private RelayCommand _generateCommand;
        public RelayCommand GenerateCommand => _generateCommand ?? (_generateCommand = new RelayCommand(Generate));

        private void Generate(object sender)
        {
            if (InputData.IsCorrect())
            {
                IsGenerated = true;
                PaymentMatrix = new PaymentMatrix(InputData, _matrix,_priceMatrix);
            }
        }

        private RelayCommand _randomFillCommand;
        public RelayCommand RandomFillCommand => _randomFillCommand ?? (_randomFillCommand = new RelayCommand(RandomFill));

        private void RandomFill(object sender)
        {
            _paymentMatrix.FillRandom(InputData);
        }

        public ApplicationViewModel(DataGrid matrix, DataGrid priceMatrix)
        {
            InputData = new InputData();
            _priceMatrix = priceMatrix;
            _matrix = matrix;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
