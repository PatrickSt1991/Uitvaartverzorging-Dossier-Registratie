using Dossier_Registratie.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dossier_Registratie.Helper
{
    public class DatumOverlijdenConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return null;

            var datumOverlijden = values[0] as DateTime?;
            var voorregeling = values[1] as bool?;

            if (voorregeling == true)
                return "Voorregeling";

            if (datumOverlijden == null && voorregeling == false)
                return "Onbekend";

            return datumOverlijden.Value.ToString("dd-MM-yyyy");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // ConvertBack not needed
            throw new NotImplementedException();
        }
    }

    public class YearToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int year)
                return year == 1 ? "Voorregelingen" : year.ToString();

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && str == "Voorregelingen")
                return 1;

            if (int.TryParse(value.ToString(), out int year))
                return year;

            return null;
        }
    }
    public class NullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value as string, out int result))
                return result;
            return null;
        }
    }
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
        private readonly NumberFormatInfo dutchNumberFormatInfo;

        public DecimalToStringConverter()
        {
            // Configure Dutch NumberFormatInfo
            dutchNumberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = ",", // Use ',' for decimals
                NumberGroupSeparator = ".",  // Use '.' for thousands
                CurrencySymbol = "€"         // Optional: Set currency symbol
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                // If value is 0, return an empty string
                if (decimalValue == 0m)
                {
                    return string.Empty;
                }
                // Format using the custom Dutch number format
                return decimalValue.ToString("N2", dutchNumberFormatInfo);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return 0m; // Return 0 for empty input
            }

            string stringValue = value.ToString().Trim();

            // Replace custom group separators with standard ones for parsing
            stringValue = stringValue.Replace(dutchNumberFormatInfo.NumberGroupSeparator, "");
            stringValue = stringValue.Replace(dutchNumberFormatInfo.NumberDecimalSeparator, ".");

            // Parse the value using invariant culture to handle the normalized format
            if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
            {
                return decimalValue;
            }

            return 0m; // Or throw an exception if parsing fails
        }
    }

}
