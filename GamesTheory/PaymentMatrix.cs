using GamesTheory.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace GamesTheory
{
    public class PaymentMatrix : INotifyPropertyChanged
    {
        private DataTable _table;
        private DataTable _priceTable;
        private static Random _rnd = new Random();
        private int _maxmin = -999, _minmax = -999;
        private DataGrid _matrix;
        private DataGrid _priceMatrix;

        private const string SubscriptDigits =
            "\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089";

        public bool IsSimplified
        {
            get => _isSimplified;
            set { _isSimplified = value; OnPropertyChanged();}
        }

        public int MaxMin
        {
            get => _maxmin;
            set
            {
                _maxmin = value;
                OnPropertyChanged();
            }
        }

        public int MinMax
        {
            get => _minmax;
            set
            {
                _minmax = value;
                OnPropertyChanged();
            }
        }

        public DataTable Table
        {
            get => _table;
            set
            {
                _table = value;
                OnPropertyChanged();
            }
        }

        public DataTable PriceTable
        {
            get => _priceTable;
            set
            {
                _priceTable = value;
                OnPropertyChanged();
            }
        }

        public SimplificationStep[] SimplifiedData
        {
            get
            {
                var list = new List<SimplificationStep>();
                bool isSimplified;
                var copy = _table.Copy();
                do
                {
                    isSimplified = false;
                    /*Перевірка стовпців*/
                    for (int i = 1; i < copy.Columns.Count; i++)
                    {
                        for (int j = i + 1; j < copy.Columns.Count; j++)
                        {
                            bool isDominant = true;
                            for (int k = 0; k < copy.Rows.Count; k++)
                            {
                                if ((int)copy.Rows[k][i] >= (int)copy.Rows[k][j])
                                {
                                    isDominant = false;
                                    break;
                                }
                            }

                            if (isDominant)
                            {
                                copy.Columns.Remove(copy.Columns[j]);
                                var step = new SimplificationStep
                                {
                                    Data = copy.Copy(),
                                    StepInfo = $"Крок {list.Count + 1}: Up2 (B{i};Sp1) > Up2(B{j};Sp1)"
                                };
                                list.Add(step);
                                isSimplified = true;
                                break;
                            }
                        }

                        if (isSimplified)
                        {
                            break;
                        }
                    }

                    /*Перевірка рядків*/
                    if (!isSimplified)
                    {
                        for (int i = 0; i < copy.Rows.Count; i++)
                        {
                            for (int j = i + 1; j < copy.Rows.Count; j++)
                            {
                                bool isDominant = true;
                                for (int k = 1; k < copy.Columns.Count; k++)
                                {
                                    if ((int)copy.Rows[i][k] <= (int)copy.Rows[j][k])
                                    {
                                        isDominant = false;
                                        break;
                                    }
                                }

                                if (isDominant)
                                {
                                    copy.Rows.Remove(copy.Rows[j]);
                                    var step = new SimplificationStep
                                    {
                                        Data = copy.Copy(),
                                        StepInfo = $"Крок {list.Count+1}: Up1 (A{i+1};Sp2) > Up1(A{j+1};Sp2)" 
                                    };
                                    list.Add(step);
                                    isSimplified = true;
                                    break;
                                }
                            }
                        }
                    }
                } while (isSimplified);

                IsSimplified = (list.Count == 0);
                return list.ToArray();
            }
        }

        public Reaction[] FirstReactions
        {
            get
            {
                var arr = new Reaction[_table.Columns.Count - 1];
                for (var i = 1; i <= arr.Length; i++)
                {
                    int max = -9999;
                    var index = new List<int>();
                    for (var j = 0; j < _table.Rows.Count; j++)
                    {
                        if (max < (int)_table.Rows[j][i])
                        {
                            max = (int)_table.Rows[j][i];
                        }
                    }
                    for (var j = 0; j < _table.Rows.Count; j++)
                    {
                        if (max == (int)_table.Rows[j][i])
                        {
                            index.Add(j);
                        }
                    }
                    arr[i - 1] = new Reaction(i, index.ToArray(), max, " A");
                }

                return arr;
            }
        }

        public Reaction[] SecondReactions
        {
            get
            {
                var arr = new Reaction[_table.Rows.Count];
                for (var i = 0; i < arr.Length; i++)
                {
                    int min = 9999;
                    var index = new List<int>();
                    for (var j = 1; j < _table.Columns.Count; j++)
                    {
                        if (min > (int)_table.Rows[i][j])
                        {
                            min = (int)_table.Rows[i][j];
                        }
                    }

                    for (var j = 1; j < _table.Columns.Count; j++)
                    {
                        if (min == (int)_table.Rows[i][j])
                        {
                            index.Add(j);
                        }
                    }
                    arr[i] = new Reaction(i + 1, index.ToArray(), -min, " B");
                }

                return arr;
            }
        }

        public PaymentMatrix(InputData data, DataGrid matrix, DataGrid priceMatrix)
        {
            _matrix = matrix;
            _priceMatrix = priceMatrix;

            _table = new DataTable();
            _table.Columns.Add("\\", typeof(string));
            for (int i = 0; i < data.StrategySecondPlayer; i++)
            {
                string subscript = new string((i + 1).ToString().Select(x => SubscriptDigits[x - '0'])
                    .ToArray());
                _table.Columns.Add($"B{subscript}", typeof(int));
            }

            for (int i = 0; i < data.StrategyFirstPlayer; i++)
            {
                var row = _table.NewRow();
                var list = new object[data.StrategySecondPlayer + 1];
                string subscript = new string((i + 1).ToString().Select(x => SubscriptDigits[x - '0'])
                    .ToArray());
                list[0] = $"A{subscript}";
                for (int j = 0; j < data.StrategySecondPlayer; j++)
                {
                    list[j + 1] = _rnd.Next(data.MinValue, data.MaxValue + 1);
                }

                row.ItemArray = list;
                _table.Rows.Add(row);
            }

            CopyTable();
            _table.RowChanged += TableOnRowChanged;

            _matrix.GridLinesVisibility = DataGridGridLinesVisibility.All;
            _matrix.VerticalGridLinesBrush = new SolidColorBrush(Color.FromRgb(73, 73, 73));
            _priceMatrix.GridLinesVisibility = DataGridGridLinesVisibility.All;
            _priceMatrix.VerticalGridLinesBrush = new SolidColorBrush(Color.FromRgb(73, 73, 73));
        }

        private void TableOnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            OnPropertyChanged("FirstReactions");
            OnPropertyChanged("SecondReactions");
            OnPropertyChanged("SimplifiedData");
            CopyTable();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void FillRandom(InputData data)
        {
            if (data.MaxValue < data.MinValue)
            {
                var t = data.MaxValue;
                data.MaxValue = data.MinValue;
                data.MinValue = t;
            }

            for (int i = 0; i < data.StrategyFirstPlayer; i++)
            {
                for (int j = 0; j < data.StrategySecondPlayer; j++)
                {
                    _table.Rows[i][j + 1] = _rnd.Next(data.MinValue, data.MaxValue + 1);
                }
            }

            CopyTable();
        }

        private void CopyTable()
        {
            _priceTable?.Clear();
            _priceTable = _table.Copy();
            _priceTable.Columns.Add("α", typeof(int));
            var row = _priceTable.NewRow();
            var list = new object[_table.Columns.Count];
            list[0] = $"β";
            row.ItemArray = list;
            _priceTable.Rows.Add(row);
            OnPropertyChanged("PriceTable");
        }

        private RelayCommand _calculatePriceCommand;
        private bool _isSimplified;
        public RelayCommand CalculatePriceCommand => _calculatePriceCommand ?? (_calculatePriceCommand = new RelayCommand(CalculatePrice));

        public void CalculatePrice(object sender)
        {
            _maxmin = -999;
            _minmax = 999;
            for (int i = 0; i < _priceTable.Rows.Count - 1; i++)
            {
                int minn = (int)_priceTable.Rows[i][1];
                for (int j = 1; j < _priceTable.Columns.Count - 2; j++)
                {
                    minn = Min(minn, (int)_priceTable.Rows[i][j]);
                }

                _priceTable.Rows[i][_priceTable.Columns.Count - 1] = minn;
                _maxmin = Max(_maxmin, minn);
            }

            for (int i = 1; i < _priceTable.Columns.Count - 1; i++)
            {
                int max = (int)_priceTable.Rows[0][i];
                for (int j = 0; j < _priceTable.Rows.Count - 1; j++)
                {
                    max = Max(max, (int)_priceTable.Rows[j][i]);
                }
                _priceTable.Rows[_priceTable.Rows.Count - 1][i] = max;
                _minmax = Min(_minmax, max);
            }
            OnPropertyChanged("MaxMin");
            OnPropertyChanged("MinMax");
        }

        private static T Min<T>(T a, T b)
        {
            return (dynamic)a > b ? b : a;
        }

        private static T Max<T>(T a, T b)
        {
            return (dynamic)a > b ? a : b;
        }
    }
}
