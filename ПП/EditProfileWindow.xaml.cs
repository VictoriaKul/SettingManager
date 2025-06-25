using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ПП
{
    public partial class EditProfileWindow : Window
    {
        // Публичные свойства для доступа к данным
        public string ProfileName { get; private set; }
        public string ProfileDescription { get; private set; }
        public bool IsDefault { get; private set; }

        public EditProfileWindow(Profile profile)
        {
            InitializeComponent();

            // Инициализируем свойства
            ProfileName = profile.Name;
            ProfileDescription = profile.Description;
            IsDefault = profile.IsDefault;

            // Привязываем данные к элементам управления
            NameTextBox.Text = ProfileName;
            DescriptionTextBox.Text = ProfileDescription;
            IsDefaultCheckBox.IsChecked = IsDefault;
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Название профиля не может быть пустым");
                return;
            }

            // Обновляем свойства перед закрытием
            ProfileName = NameTextBox.Text;
            ProfileDescription = DescriptionTextBox.Text;
            IsDefault = IsDefaultCheckBox.IsChecked ?? false;

            DialogResult = true;
            Close();
        }
    
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
