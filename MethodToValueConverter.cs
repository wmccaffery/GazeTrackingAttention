using System;
//Based on stackoverflow answer to question "Bind to a method in WPF?" posted by Cameron MacFarland on Febuary 2nd 2009
//Author: Drew Noakes, May 10th 2009
//Retrieved from: https://stackoverflow.com/questions/502250/bind-to-a-method-in-wpf on 16/10/18
public class MethodToValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var methodName = parameter as string;
        if (value == null || methodName == null)
            return value;
        var methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
        if (methodInfo == null)
            return value;
        return methodInfo.Invoke(value, new object[0]);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
    }
}
