namespace FINE.Utilities.WPF;

using System;
using System.Windows.Data;

public class BoolToZIndexConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => ((bool)value) ? 1 : 0;

  public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
}
