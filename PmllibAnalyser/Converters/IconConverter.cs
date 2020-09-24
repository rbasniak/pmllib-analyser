using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PmllibAnalyser
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PmllibFolder)
            {
                if ((value as PmllibFolder).IsMigrated)
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/green_folder.png"));
                }
                else
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/red_folder.png"));
                }
            }
            else if (value is PmllibFile)
            {
                var file = value as PmllibFile;

                if (Pmllib.IGNORED_EXTENSIONS.Any(x => x == file.File.Extension.ToLower()) && file.File.Extension != "")
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/non-code.png"));
                }
                else
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/code.png"));
                }
            }
            else if (value is Uda)
            {
                return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/uda.png"));
            }
            else if (value is TreeNode)
            {
                var node = value as TreeNode;
                
                if (node.Title[0] == ':')
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/uda.png"));
                }
                else if (node.Children != null && node.Children.Count > 0)
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/folder.png"));
                }
                else
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/PmllibAnalyser;component/Resources/code.png"));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
