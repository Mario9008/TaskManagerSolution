using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManagerLibrary;

namespace TaskManagerUI.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskManagerLibrary.TaskStatus status)
            {
                return status switch
                {
                    TaskManagerLibrary.TaskStatus.Новая => new SolidColorBrush(Color.FromRgb(108, 92, 231)),
                    TaskManagerLibrary.TaskStatus.ВПроцессе => new SolidColorBrush(Color.FromRgb(253, 203, 110)),
                    TaskManagerLibrary.TaskStatus.Завершена => new SolidColorBrush(Color.FromRgb(0, 184, 148)),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}