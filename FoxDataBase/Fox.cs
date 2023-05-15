namespace FoxDataBase;

/// <summary>
/// Описывает лису.
/// </summary>
public class Fox
{
    // Вид лисы.
    private string _species = "Undefined";

    // Изображение лисы.
    private byte[]? _image;


    /// <value>
    /// Длина хвоста в сантиметрах.
    /// </value>
    public int? _tailLength;


    /// <value>
    /// Описание вида.
    /// </value>
    public string? Description { get; set; }


    /// <summary>
    /// Инициализирует вид лисы строкой "Undefined",
    /// другие свойства - значением null.
    /// </summary>
    public Fox() : this("Undefined", null, null, null) { }


    /// <summary>
    /// Инициализирует лису всему свойствами.
    /// </summary>
    /// <param name="species">Вид лисы.</param>
    /// <param name="description">Описание.</param>
    /// <param name="tailLenght">Длина хвоста.</param>
    /// <param name="image">Изображение лисы.</param>
    public Fox(string species,
               string? description,
               int? tailLenght,
               byte[]? image)
    {
        Species = species;
        Description = description;
        TailLength = tailLenght;
        Image = image;
    }


    /// <value>
    /// Вид лисы.
    /// </value>
    /// <exception cref="ArgumentException"></exception>
    public string Species
    {
        get => _species;

        set
        {
            value = value.TrimStart();

            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Species cannot be empty or null");

            _species = value;
        }
    }


    /// <value>
    /// Длина хвоста.
    /// </value>
    /// <exception cref="ArgumentException"></exception>
    public int? TailLength
    {
        get => _tailLength;

        set
        {
            if (value <= 0)
                throw new ArgumentException("Tail length cannot be <= 0");

            _tailLength = value;
        }
    }


    /// <summary>
    /// Картинка лисы.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public byte[]? Image
    {
        get => _image;

        set
        {
            if (Image?.Length == 0)
                throw new ArgumentException("Image data cannot be empty");

            _image = value;
        }
    }
}