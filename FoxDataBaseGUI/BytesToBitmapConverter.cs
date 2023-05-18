using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FoxDataBaseGUI;

/// <summary>
/// Конвертер из массива байт в bitmap изображение.
/// </summary>
public class BytesToBitmapConverter : IValueConverter
{
    /// <summary>
    /// Конвертировать из массива байт в bitmap изображение.
    /// </summary>
    /// <param name="value">Данные изображения в массиве байт.</param>
    /// <param name="targetType">Не используется.</param>
    /// <param name="parameter">Выходной размер изображения в пикселях (int).</param>
    /// <param name="culture">Не используется.</param>
    /// <returns></returns>
    public object? Convert(object value,
                           Type targetType,
                           object parameter,
                           System.Globalization.CultureInfo culture)
    {
        if (value == null) return null;

        byte[] imageData = (byte[])value;
        int decodePixelWidth = 200;

        if (parameter != null)
            decodePixelWidth = System.Convert.ToInt32(parameter);
        
        return GetBitmapImageFromBytes(imageData, decodePixelWidth);
    }


    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Конвертировать из массива байт в bitmap изображение.
    /// </summary>
    /// <param name="imageData">Данные изображения.</param>
    /// <param name="decodeWidth">Выходной размер изображения в пикселях.</param>
    /// <returns>Bitmap изображение.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static BitmapImage GetBitmapImageFromBytes(byte[] imageData, int decodeWidth)
    {
        if (imageData.Length == 0)
            throw new ArgumentException("Image data cannot be empty");
        if (decodeWidth <= 0)
            throw new ArgumentException("Decode width cannot be <= 0");

        BitmapImage image = new();
        using MemoryStream memory = new(imageData);

        memory.Position = 0;
        image.BeginInit();
        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = null;
        image.StreamSource = memory;
        image.DecodePixelWidth = decodeWidth;
        image.EndInit();
        image.Freeze();

        return image;
    }
}
