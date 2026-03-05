namespace NodeNetwork.Utilities.WPF;

using System;
using System.Windows;
using System.Windows.Data;

public class NullVisibilityConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value == null ? Visibility.Collapsed : Visibility.Visible;

  public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
}
