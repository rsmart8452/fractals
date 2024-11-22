using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Setup.BA.Util
{
  /// <summary>
  ///   Converts a <see cref="bool" /> to its NOT (opposite) value.
  /// </summary>
  [ValueConversion(typeof(bool), typeof(bool))]
  public class BooleanNotConverter : MarkupExtension, IValueConverter
  {
    /// <summary>
    ///   Converts a value.
    /// </summary>
    /// <returns>
    ///   A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var result = !(value as bool?);
      if (targetType == typeof(bool?))
        return result;

      if (targetType == typeof(bool))
        return result != null && result.Value;

      return null;
    }

    /// <summary>
    ///   Converts a value.
    /// </summary>
    /// <returns>
    ///   A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Convert(value, targetType, parameter, culture);
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
}