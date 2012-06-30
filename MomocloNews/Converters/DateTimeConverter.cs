using System;
using System.Globalization;
using System.Windows.Data;

namespace MomocloNews.Converters
{
    public class DateTimeToTimelineFormatStringConverter : IValueConverter
    {
        private const string FORMAT = "g";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToString(FORMAT);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeArrayToDateTimeFromToFormatStringConverter : IValueConverter
    {
        private const string DATE_FORMAT = "d";
        private const string DATETIME_FORMAT = "g";
        private const string TIME_FORMAT = "t";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime[])
            {
                DateTime[] dates = (DateTime[])value;
                DateTime start = dates[0];
                DateTime end = dates[1];
                TimeSpan span = end - start;
                if (span.Days == 1 && start.TimeOfDay == end.TimeOfDay)
                {
                    return start.ToString(DATE_FORMAT);
                }
                if (span.Days == 0)
                {
                    return
                        string.Format(
                            Localization.AppResources.Common_DateTimeFromToFormat_FromTo,
                            start.ToString(DATETIME_FORMAT),
                            end.ToString(TIME_FORMAT)
                        );
                }
                else
                {
                    return
                        string.Format(
                            Localization.AppResources.Common_DateTimeFromToFormat_FromTo,
                            start.ToString(DATETIME_FORMAT),
                            end.ToString(DATETIME_FORMAT)
                        );
                }
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
