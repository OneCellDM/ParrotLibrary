using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ParrotLibrary
{
   public class ImageDataValueConverter : IValueConverter
    {
        public static BitmapImage DefaultImage=new BitmapImage( new Uri( "pack://application:,,,/Res/NoImage.jpeg", UriKind.RelativeOrAbsolute));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if(value==null)
                    return DefaultImage;

                if (new FileInfo((string)value).Exists)
                {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(value.ToString());
                        image.DecodePixelWidth = 200;
                        image.DecodePixelHeight = 320;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.CreateOptions = BitmapCreateOptions.None;      
                        image.EndInit();       
                        return image;
                }
               
            }
            catch (System.NotSupportedException ex) {  }

            return DefaultImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
