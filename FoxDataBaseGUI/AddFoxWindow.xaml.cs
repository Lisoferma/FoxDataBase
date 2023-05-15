using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FoxDataBaseGUI;

using FoxDataBase;

/// <summary>
/// Логика взаимодействия для AddFoxWindow.xaml.
/// Окно для ввода данных лисы и добавления в базу данных.
/// </summary>
public partial class AddFoxWindow : Window
{
    // База данных куда будут добавляться новые лисы.
    private readonly FoxDataBase _dbFox;

    // Данные лисы вводимые пользователем.
    private Fox _fox;

    // Ключ для нахождения лисы в базе данных
    // по атрибуту species и последующего редактирования.
    private readonly string? _speciesToUpdate;


    /// <summary>
    /// Окно для ввода данных лисы и добавления в базу данных.
    /// </summary>
    /// <param name="dbFox">База данных в которую будут добавляться новые данные.</param>
    public AddFoxWindow(FoxDataBase dbFox)
    {
        InitializeComponent();
        _dbFox = dbFox;
        _fox = new();

        ButtonApply.Content = "Добавить";
    }


    /// <summary>
    /// Окно для ввода данных лисы и добавления в базу данных.
    /// </summary>
    /// <param name="dbFox">База данных в которую будут добавляться новые данные.</param>
    public AddFoxWindow(FoxDataBase dbFox, Fox editFox)
    {
        InitializeComponent();
        _dbFox = dbFox;
        _fox = editFox;
        _speciesToUpdate = editFox.Species;
        DataContext = _fox;

        ButtonApply.Content = "Изменить";    
    }


    /// <summary>
    /// Обработчик нажатия на кнопку <see cref="ButtonAdd"/>.
    /// Если все поля заполнены, добавляет новую лису в базу данных.
    /// </summary>
    private void ButtonApply_Click(object sender, RoutedEventArgs e)
    {
        if (IsTextBoxesHasError())
        {
            MessageBox.Show("Ошибка: не все поля заполнены корректно");
            return;
        }

        try
        {
            ParseTextboxesToFox();

            if (_speciesToUpdate != null)
            {
                _dbFox.Update(_fox, _speciesToUpdate);
                Close();
                return;
            }
            else
            {
                _dbFox.Insert(_fox);
            }              
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
            return;
        }

        ClearAllInput();
    }


    /// <summary>
    /// Проверяет имеются ли ошибки у полей.
    /// </summary>
    /// <returns>
    /// true - имеются ошибки,
    /// false - ошибок нет.
    /// </returns>
    private bool IsTextBoxesHasError()
    {
        if (Validation.GetHasError(TextBoxSpecies)
            || Validation.GetHasError(TextBoxTailLength)
            || Validation.GetHasError(TextBoxDescription))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Перевести данные из текстбоков в свойства лисы.
    /// </summary>
    private void ParseTextboxesToFox()
    {
        _fox.Species = TextBoxSpecies.Text;

        if (TextBoxTailLength.Text == "")
            _fox.TailLength = null;
        else
            _fox.TailLength = int.Parse(TextBoxTailLength.Text);

        if (TextBoxDescription.Text == "")
            _fox.Description = null;
        else
            _fox.Description = TextBoxDescription.Text;
    }


    /// <summary>
    /// Очистить все поля ввода.
    /// </summary>
    private void ClearAllInput()
    {
        TextBoxSpecies.Text = "Undefined";
        TextBoxTailLength.Clear();
        TextBoxDescription.Clear();
        Image_UserLoad.Source =
            new BitmapImage(new Uri("Images/Fox.png", UriKind.Relative));
    }


    /// <summary>
    /// Убирает текст внутри <see cref="TextBoxSpecies"/> при фокусе на нём,
    /// если текст равен Undefined.
    /// </summary>
    private void TextBoxSpecies_GotFocus(object sender, RoutedEventArgs e)
    {
        if (TextBoxSpecies.Text == "Undefined")
            TextBoxSpecies.Text = "";
    }


    /// <summary>
    /// Заменяет текст <see cref="TextBoxTailLength"/> при изменении на null,
    /// если поле пустое, чтобы не было подсветки ошибки.
    /// </summary>
    private void TextBoxTailLength_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (TextBoxTailLength.Text == "")
            TextBoxTailLength.Text = null;
    }


    /// <summary>
    /// Обработчик нажатия на <see cref="Image_UserLoad"/>.
    /// Загрузка изображения пользователем.
    /// </summary>
    private void Image_UserLoad_MouseDown
        (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Filter = "Image files|*.png;*.jpg;*.jpeg|All(*.*)|*"
        };

        bool? isDialogShown = dialog.ShowDialog();

        if (isDialogShown == false ||
            isDialogShown == null) return;

        byte[] imageData;

        try
        {
            using (FileStream fs = new(dialog.FileName, FileMode.Open))
            {
                imageData = new byte[fs.Length];
                fs.Read(imageData, 0, imageData.Length);
            }

            Image_UserLoad.Source =
                new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute));    
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
            return;
        }
     
        _fox.Image = imageData;
        MessageBox.Show($"Байт: {_fox.Image?.Length}");
    }
}

