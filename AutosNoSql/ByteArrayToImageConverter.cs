using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutosNoSql
{
    class ByteArrayToImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Verificar si el valor es un arreglo de bytes
            if (value is byte[] byteArray && byteArray.Length > 0)
            {
                // Convertir el arreglo de bytes en una imagen
                ImageSource imageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(byteArray));
                return imageSource;
            }
            // Si el valor no es un arreglo de bytes o está vacío, retornar nulo
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Este método no se usa para la conversión de bytes a imagen
            throw new NotImplementedException();
        }

    }
}
