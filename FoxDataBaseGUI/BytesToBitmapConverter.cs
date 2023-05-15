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
    public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null) return null;

        byte[] imageData = (byte[])value;
        return GetBitmapImageFromBytes(imageData);
    }


    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Конвертировать из массива байт в bitmap изображение.
    /// </summary>
    /// <param name="imageData">Данные изображения.</param>
    /// <returns>Bitmap изображение.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static BitmapImage GetBitmapImageFromBytes(byte[] imageData)
    {
        if (imageData.Length == 0)
            throw new ArgumentException("Image data cannot be empty");

        BitmapImage image = new();
        using MemoryStream mem = new(imageData);

        mem.Position = 0;
        image.BeginInit();
        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = null;
        image.StreamSource = mem;
        image.DecodePixelWidth = 200;
        image.DecodePixelHeight = 200;
        image.EndInit();

        image.Freeze();
        return image;
    }
}
