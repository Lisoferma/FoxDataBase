using System.Windows;

namespace FoxDataBaseGUI
{
    /// <summary>
    /// Логика взаимодействия для окна HelpWindow.xaml
    /// Окно справки.
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Обработчик нажатия на кнопку <see cref="ButtonOk"/>.
        /// Закрывает окно.
        /// </summary>
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
