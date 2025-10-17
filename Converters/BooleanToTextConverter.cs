using System.Globalization;

namespace StockApp.Converters
{
    public class BooleanToTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string textParameter)
            {
                var texts = textParameter.Split(';');
                if (texts.Length == 2)
                {
                    return boolValue ? texts[0] : texts[1];
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}