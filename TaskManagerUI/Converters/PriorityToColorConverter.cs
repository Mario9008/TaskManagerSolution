using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManagerLibrary;

namespace TaskManagerUI.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority priority)
            {
                return priority switch
                {
                    TaskPriority.Высокий => new SolidColorBrush(Color.FromRgb(214, 48, 49)),
                    TaskPriority.Средний => new SolidColorBrush(Color.FromRgb(253, 203, 110)),
                    TaskPriority.Низкий => new SolidColorBrush(Color.FromRgb(0, 184, 148)),
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