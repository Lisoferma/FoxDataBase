using System.IO;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace FoxDataBase;

/// <summary>
/// База данных для типа <see cref="Fox"/>, построенная на SQLite.
/// </summary>
public class FoxDataBase
{
    // Содержит данные лис.
    private ObservableCollection<Fox> _foxData;

    /// <summary>
    /// Содержит данные лис.
    /// </summary>
    public ReadOnlyObservableCollection<Fox> FoxData;

    // Путь к файлу базы данных.
    private readonly string _filePath;


    /// <summary>
    /// Инициализировать базу данных с названием Foxes.db в папке программы.
    /// </summary>
    public FoxDataBase() : this("Foxes.db") { }


    /// <summary>
    /// Инициализировать базу данных и задать путь.
    /// </summary>
    /// <param name="filePath">Путь к файлу базы данных.</param>
    /// <exception cref="ArgumentException"></exception>
    public FoxDataBase(string filePath)
    {
        if (filePath == "")
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        _filePath = filePath;

        _foxData = new ObservableCollection<Fox>();
        FoxData = new ReadOnlyObservableCollection<Fox>(_foxData);
    }


    /// <value>
    /// Название файла базы данных.
    /// </value>
    public string Filename
    {
        get => _filePath.Substring(_filePath.LastIndexOf('\\') + 1);
    }


    /// <summary>
    /// Создать файл базы данных.
    /// </summary>
    public void CreateTable()
    {
        const string sqlCreatTable =
            @"CREATE TABLE foxes(
                species     TEXT PRIMARY KEY,
                description TEXT NULL,
                tail_length INTEGER NULL,
                image       BLOB NULL)";

        string connectionString =
            $"Data Source={_filePath};Mode=ReadWriteCreate;";


        using SqliteConnection connection = new(connectionString);
        connection.Open();

        SqliteCommand command = new()
        {
            Connection = connection,
            CommandText = sqlCreatTable
        };

        try
        {
            command.ExecuteNonQuery();
        }
        catch
        {
            File.Delete(_filePath);
        }    
    }


    /// <summary>
    /// Загрузить базу данных.
    /// </summary>
    public void Load()
    {
        const string sqlSelectAll = "SELECT * FROM foxes";

        string connectionString =
            $"Data Source={_filePath};Mode=ReadOnly;";


        using SqliteConnection connection = new(connectionString);
        connection.Open();

        SqliteCommand command = new()
        {
            Connection = connection,
            CommandText = sqlSelectAll
        };

        using SqliteDataReader reader = command.ExecuteReader();


        if (!reader.HasRows) return;

        while (reader.Read())   // построчно считывает данные
        {
            string species =
                reader.GetString(0);

            string? description =
                reader.IsDBNull(1) ? null : reader.GetString(1);

            int? tailLength =
                reader.IsDBNull(2) ? null : reader.GetInt32(2);

            byte[]? image =
                reader.IsDBNull(3) ? null : (byte[]?)reader.GetValue(3);

            Fox fox = new(species, description, tailLength, image);
            _foxData.Add(fox);
        }
    }


    /// <summary>
    /// Добавить элемент в базу данных.
    /// </summary>
    /// <param name="fox">Элемент который нужно добавить.</param>
    public void Insert(Fox fox)
    {
        const string sqlInsert =
            @"INSERT INTO foxes (species,
                                 description,
                                 tail_length,
                                 image)
              VALUES (@species,
                      @description,
                      @tail_length,
                      @image)";

        string connectionString =
            $"Data Source={_filePath};Mode=ReadWrite;";


        using SqliteConnection connection = new(connectionString);
        connection.Open();

        SqliteCommand command = new()
        {
            Connection = connection,
            CommandText = sqlInsert
        };

        command.Parameters.Add(
            new SqliteParameter("@species", fox.Species));

        command.Parameters.Add(
            new SqliteParameter("@description", fox.Description ?? (object)DBNull.Value));

        command.Parameters.Add(
            new SqliteParameter("@tail_length", fox.TailLength ?? (object)DBNull.Value));

        command.Parameters.Add(
            new SqliteParameter("@image", fox.Image ?? (object)DBNull.Value));

        command.ExecuteNonQuery();

        _foxData.Add(fox);
    }


    /// <summary>
    /// Отредактировать лису в базе данных.
    /// </summary>
    /// <param name="editedFox">Отредактированная лиса.</param>
    /// <param name="oldSpecies">Прошлое свойство species редактируемой лисы.</param>
    public void Update(Fox editedFox, string oldSpecies)
    {
        const string sqlUpdate =
            @$"UPDATE foxes
               SET species     = @species,
                   description = @description,
                   tail_length = @tail_length,
                   image       = @image
               WHERE species = @old_species";

        string connectionString =
            $"Data Source={_filePath};Mode=ReadWrite;";


        using SqliteConnection connection = new(connectionString);
        connection.Open();

        SqliteCommand command = new()
        {
            Connection = connection,
            CommandText = sqlUpdate
        };

        command.Parameters.Add(
            new SqliteParameter("@old_species", oldSpecies));

        command.Parameters.Add(
            new SqliteParameter("@species", editedFox.Species));

        command.Parameters.Add(
            new SqliteParameter("@description", editedFox.Description ?? (object)DBNull.Value));

        command.Parameters.Add(
            new SqliteParameter("@tail_length", editedFox.TailLength ?? (object)DBNull.Value));

        command.Parameters.Add(
            new SqliteParameter("@image", editedFox.Image ?? (object)DBNull.Value));

        command.ExecuteNonQuery();

        ReplaceItem(editedFox, oldSpecies);
    }


    /// <summary>
    /// Удалить лису из базы данных по свойству species.
    /// </summary>
    /// <param name="speciesToDelete">Свойство species лисы которую нужно удалить.</param>
    public void Delete(string speciesToDelete)
    {
        const string sqlDelete =
            "DELETE FROM foxes WHERE species = @species";

        string connectionString =
            $"Data Source={_filePath};Mode=ReadWrite;";


        using SqliteConnection connection = new(connectionString);
        connection.Open();

        SqliteCommand command = new()
        {
            Connection = connection,
            CommandText = sqlDelete
        };

        command.Parameters.Add(
            new SqliteParameter("@species", speciesToDelete));

        command.ExecuteNonQuery();

        DeleteItem(speciesToDelete);
    }


    /// <summary>
    /// Заменить лису в <see cref="_foxData"/> на новую.
    /// </summary>
    /// <param name="newFox">Новая лиса.</param>
    /// <param name="speciesToReplace">Заменить лису с таким свойством species.</param>
    private void ReplaceItem(Fox newFox, string speciesToReplace)
    {
        for (int i = 0; i <= _foxData.Count - 1; i++)
        {
            if (_foxData[i].Species == speciesToReplace)
            {
                _foxData[i] = newFox;
                break;
            }
        }
    }


    /// <summary>
    /// Удалить лису в <see cref="_foxData"/>.
    /// </summary>
    /// <param name="speciesToDelete">Удалить лису с таким свойством species.</param>
    private void DeleteItem(string speciesToDelete)
    {
        for (int i = 0; i <= _foxData.Count - 1; i++)
        {
            if (_foxData[i].Species == speciesToDelete)
            {
                _foxData.RemoveAt(i);
                break;
            }
        }
    }
}
