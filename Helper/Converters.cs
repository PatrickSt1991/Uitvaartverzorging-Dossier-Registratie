using Dossier_Registratie.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dossier_Registratie.Helper
{
    public class VerzekeraarsModelToPolisVerzekeringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VerzekeraarsModel verzekeraarsModel)
            {
                return new PolisVerzekering { VerzekeringName = verzekeraarsModel.Name };
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }

            return true;
        }
    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }

            return false;
        }
    }
    public class StringNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public static class RangeUtility
    {
        public static bool IsInRange(string value, string start, string end)
        {
            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
            {
                return true;
            }

            if (long.TryParse(value, out long numericValue) &&
                long.TryParse(start, out long numericStart) &&
                long.TryParse(end, out long numericEnd))
            {
                return numericValue >= numericStart && numericValue <= numericEnd;
            }

            return false;
        }
    }
    public class StringToDecimalPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Handling percentage values
                if (stringValue.Contains("%"))
                {
                    if (decimal.TryParse(stringValue.Replace("%", ""), NumberStyles.Any, culture, out decimal result))
                    {
                        return result / 100; // Convert percentage to decimal
                    }
                }
                // Handling currency values
                else if (decimal.TryParse(stringValue, NumberStyles.Currency, culture, out decimal currencyResult))
                {
                    return currencyResult;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                // Convert decimal back to percentage string
                if (targetType == typeof(string) && parameter is string format && format == "Percentage")
                {
                    return decimalValue == 0 ? "" : $"{decimalValue:P0}";
                }
                // Convert decimal back to currency string
                else
                {
                    return decimalValue.ToString("C", culture); // Currency format
                }
            }

            return value;
        }
    }
    public class RadioButtonValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return parameter?.ToString();
            return null;
        }
    }
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return string.Format(culture, "€ {0:N2}", decimalValue);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                stringValue = stringValue.Replace("€", "").Trim(); // Remove currency symbol and trim
                if (decimal.TryParse(stringValue, NumberStyles.Number, culture, out decimal result))
                {
                    return result;
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                // Format as a string using the specified culture
                return decimalValue.ToString("N2", culture); // N2 formats to two decimal places
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is null or empty
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return 0m; // Return 0 for empty input or handle as appropriate
            }

            // Get the string value
            string stringValue = value.ToString().Trim();

            // Use Dutch culture to parse the input
            // Dutch culture uses ',' as the decimal separator
            if (decimal.TryParse(stringValue, NumberStyles.Any, new CultureInfo("nl-NL"), out decimal decimalValue))
            {
                return decimalValue;
            }

            // If parsing fails, return 0 or handle the error as appropriate
            return 0m; // Or throw an exception if invalid
        }
    }
}
