using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GamesTheory
{
    public class BackgroundCellConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var cell = (DataGridCell)values[0];
                if (cell == null)
                {
                    return null;
                }

                var row = (DataRow)values[1];
                var minmax = (int)values[2];
                var maxmin = (int)values[3];
                var table = row.Table;
                var val = (int)row[cell.Column.DisplayIndex];
                var rowIndex = table.Rows.IndexOf(row);
                if (rowIndex == table.Rows.Count - 1)
                {
                    if (val == minmax)
                    {
                        return new SolidColorBrush(Color.FromRgb(20, 255, 20));
                    }
                }

                if (table.Columns.Count - 1 == cell.Column.DisplayIndex)
                {
                    if (val == maxmin)
                    {
                        return new SolidColorBrush(Color.FromRgb(20, 255, 20));
                    }
                }

                if (minmax == maxmin)
                {
                    if (val == maxmin && (int)table.Rows[rowIndex][table.Columns.Count-1] == val && (int)table.Rows[table.Rows.Count-1][cell.Column.DisplayIndex] == val)
                    {
                        return new SolidColorBrush(Color.FromRgb(255, 0, 20));
                    }
                }

                return DependencyProperty.UnsetValue;
            }
            catch (InvalidCastException e)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
