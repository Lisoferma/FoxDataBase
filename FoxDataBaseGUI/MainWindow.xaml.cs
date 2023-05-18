using System;
using System.Linq;
using System.Windows;

namespace FoxDataBaseGUI;

using FoxDataBase;

/// <summary>
/// Логика взаимодействия для главного окна программы MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{   
    // База данных лис.
    private FoxDataBase? _dbFox;


    public MainWindow()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Обработчик нажатия на кнопку меню <see cref="MenuItem_Create"/>.
    /// Создаёт базу данных по указанному пользователем пути.
    /// </summary>
    private void MenuItem_Create_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            FileName = "Foxes",
            DefaultExt = ".db",
            Filter = "Fox database (.db)|*.db|All(*.*)|*"
        };

        bool? isDialogShown = dialog.ShowDialog();

        if (isDialogShown == false ||
            isDialogShown == null) return;

        try
        {
            _dbFox = new(dialog.FileName);
            _dbFox.CreateTable();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
            return;
        }
      
        DataGridFoxes.ItemsSource = _dbFox.FoxData;
        ButtonAdd.IsEnabled = true;
        TextBlock_Filename.Text = _dbFox.Filename;
    }


    /// <summary>
    /// Обработчик нажатия на кнопку меню <see cref="MenuItem_Open"/>.
    /// Открывает базу данных по указанному пользователем пути.
    /// </summary>
    private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            FileName = "Foxes",
            DefaultExt = ".db",
            Filter = "Fox database (.db)|*.db|All(*.*)|*"
        };

        bool? isDialogShown = dialog.ShowDialog();

        if (isDialogShown == false ||
            isDialogShown == null) return;

        try
        {
            _dbFox = new(dialog.FileName);
            _dbFox.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
            return;
        }

        DataGridFoxes.ItemsSource = _dbFox.FoxData;
        ButtonAdd.IsEnabled = true;
        TextBlock_Filename.Text = _dbFox.Filename;
    }


    /// <summary>
    /// Обработчик нажатия на кнопку меню <see cref="MenuItem_Help"/>.
    /// Открывает справку.
    /// </summary>
    private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
    {
        HelpWindow helpWindow = new();
        helpWindow.Owner = this;
        helpWindow.ShowDialog();
    }


    /// <summary>
    /// Обработчик нажатия на кнопку <see cref="ButtonAdd"/>.
    /// Открывает окно для добавления новой лисы в базу данных.
    /// </summary>
    private void ButtonAdd_Click(object sender, RoutedEventArgs e)
    {
        if (_dbFox == null) return;

        AddFoxWindow addFoxWindow = new(_dbFox);
        addFoxWindow.Owner = this;
        addFoxWindow.Title = "Добавить лису в лисопедию";
        addFoxWindow.ShowDialog();    
    }


    /// <summary>
    /// Обработчик нажатия на кнопку <see cref="ButtonEdit"/>.
    /// Открывает окно для редактирования выбранной лисы.
    /// </summary>
    private void ButtonEdit_Click(object sender, RoutedEventArgs e)
    {
        if (_dbFox == null) return;
        if (DataGridFoxes.SelectedItem == null) return;

        Fox selectedFox = (Fox)DataGridFoxes.SelectedItem;

        AddFoxWindow addFoxWindow = new(_dbFox, selectedFox);
        addFoxWindow.Owner = this;
        addFoxWindow.Title = "Изменить лису в лисопедии";
        addFoxWindow.ShowDialog();
    }


    /// <summary>
    /// Обработчик нажатия на кнопку <see cref="ButtonDelete"/>.
    /// Удаляет выбранную лису.
    /// </summary>
    private void ButtonDelete_Click(object sender, RoutedEventArgs e)
    {
        if (_dbFox == null) return;
        if (DataGridFoxes.SelectedItem == null) return;

        Fox selectedFox = (Fox)DataGridFoxes.SelectedItem;

        string sMessageBoxText = $"Вы уверены, что хотите удалить: {selectedFox.Species}?";
        string sCaption = "Предупреждение";

        MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
        MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

        MessageBoxResult rsltMessageBox =
            MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

        switch (rsltMessageBox)
        {
            case MessageBoxResult.Yes:
                try
                {
                    _dbFox.Delete(selectedFox.Species);
                    MessageBox.Show($"{selectedFox.Species} удалён");
                    DataGridFoxes.UpdateLayout();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
                break;

            case MessageBoxResult.No:
                return;
        }
    }


    /// <summary>
    /// При фокусе на <see cref="DataGridFoxes"/>.
    /// Делает доступными кнопки для редактирования и удаления лисы.
    /// </summary>
    private void DataGridFoxes_GotFocus(object sender, RoutedEventArgs e)
    {
        ButtonEdit.IsEnabled = true;
        ButtonDelete.IsEnabled = true;
    }


    /// <summary>
    /// Обработчик изменения текста в <see cref="TextBoxSearch"/>.
    /// Поиск по введённому тексту в <see cref="DataGridFoxes"/>.
    /// </summary>
    private void TextBoxSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (TextBoxSearch.Text != "")
        {
            string filter = TextBoxSearch.Text.ToLower();

            var filteredList =
                _dbFox?.FoxData.Where( fox => fox.Species.ToLower().Contains(filter) );

            DataGridFoxes.ItemsSource = filteredList;
        }
        else
        {
            DataGridFoxes.ItemsSource = _dbFox?.FoxData;
        }
    }
}
