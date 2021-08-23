using System;
using System.Globalization;
using System.Windows.Data;
using JdaTools.Studio.Models;
using MahApps.Metro.IconPacks;

namespace JdaTools.Studio.Converters
{
    public class FileToContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value?.GetType();
            if (type == typeof(MocaDirectory))
            {
                return new PackIconMaterial()
                {
                    Kind = PackIconMaterialKind.FolderOutline
                };
            }
            return new PackIconMaterial()
            {
                Kind = PackIconMaterialKind.FileOutline
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}