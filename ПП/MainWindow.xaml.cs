using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ПП
{
    public partial class MainWindow : Window
    {
        private DatabaseManager dbManager;
        private ObservableCollection<Profile> profiles; // <- Добавьте это поле
        private ObservableCollection<StartupItem> startupItems;
        private ObservableCollection<AutoAction> autoActions;

        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
            LoadData();

            MonitoringTab.DataContext = new MonitoringViewModel();
        }

        private void LoadData()
        {
            // Загрузка данных автозагрузки
            startupItems = dbManager.GetStartupItems();
            StartupItemsGrid.ItemsSource = startupItems;

            // Загрузка профилей
            profiles = dbManager.GetProfiles();
            ProfilesListBox.ItemsSource = profiles;

            // Загрузка автоматических действий
            autoActions = dbManager.GetAutoActions();
            AutoActionsGrid.ItemsSource = autoActions;
        }

        // Обработчики для вкладки автозагрузки
        private void StartupItemsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedItem = e.Row.Item as StartupItem;
                dbManager.UpdateStartupItem(editedItem);
            }
        }

        // Обработчики для вкладки профилей
        private void AddProfile_Click(object sender, RoutedEventArgs e)
        {
            var newProfile = new Profile { Name = "Новый профиль" };
            dbManager.AddProfile(newProfile);
            profiles.Add(newProfile);
            ProfilesListBox.SelectedItem = newProfile;
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is Profile selectedProfile)
            {
                dbManager.DeleteProfile(selectedProfile.Id);
                profiles.Remove(selectedProfile);
            }
        }
        // Сохранение системных параметров
        private void SaveSystemSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = new SystemSettings
                {
                    Theme = ThemeComboBox.SelectedValue.ToString(),
                    Language = LanguageComboBox.SelectedValue.ToString(),
                    AutoUpdates = AutoUpdatesCheckBox.IsChecked ?? false
                };

                using (var db = new AppDbContext())
                {
                    // Удаляем старые настройки если они есть
                    var oldSettings = db.SystemSettings.ToList();
                    db.SystemSettings.RemoveRange(oldSettings);

                    db.SystemSettings.Add(settings);
                    db.SaveChanges();
                }

                MessageBox.Show("Параметры сохранены успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
        private void ProfilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ваша логика при изменении выбора профиля
            if (ProfilesListBox.SelectedItem is Profile selectedProfile)
            {
                // Пример: показываем информацию о выбранном профиле
                MessageBox.Show($"Выбран профиль: {selectedProfile.Name}\n" +
                              $"Описание: {selectedProfile.Description}\n" +
                              $"Дата создания: {selectedProfile.CreatedDate.ToShortDateString()}",
                              "Информация о профиле");

                // Или обновляем другие элементы интерфейса
                // UpdateUIWithProfileDetails(selectedProfile);
            }
        }
        // Редактирование профиля
        //private void EditProfile_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ProfilesListBox.SelectedItem == null)
        //    {
        //        MessageBox.Show("Выберите профиль для редактирования");
        //        return;
        //    }

        //    var selectedProfile = (Profile)ProfilesListBox.SelectedItem;

        //    try
        //    {
        //        var editWindow = new EditProfileWindow(selectedProfile);
        //        if (editWindow.ShowDialog() == true)
        //        {
        //            using (var db = new AppDbContext())
        //            {
        //                var profile = db.Profiles.Find(selectedProfile.Id);

        //                if (profile == null)
        //                {
        //                    MessageBox.Show("Профиль не найден в базе данных");
        //                    return;
        //                }

        //                // Обновляем только изменяемые поля
        //                profile.Name = editWindow.ProfileName ?? profile.Name;
        //                profile.Description = editWindow.ProfileDescription ?? profile.Description;
        //                profile.IsDefault = editWindow.IsDefault;

        //                db.SaveChanges();
        //                RefreshProfilesList();

        //                MessageBox.Show("Профиль успешно обновлён");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка при редактировании профиля: {ex.Message}");
        //    }
        //}
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите профиль для редактирования");
                return;
            }

            var selectedProfile = (Profile)ProfilesListBox.SelectedItem;

            try
            {
                var editWindow = new EditProfileWindow(selectedProfile);
                if (editWindow.ShowDialog() == true)
                {
                    // Обновляем только изменяемые поля
                    selectedProfile.Name = editWindow.ProfileName ?? selectedProfile.Name;
                    selectedProfile.Description = editWindow.ProfileDescription ?? selectedProfile.Description;
                    selectedProfile.IsDefault = editWindow.IsDefault;

                    // Используем dbManager вместо нового контекста
                    dbManager.UpdateProfile(selectedProfile);
                    RefreshProfilesList();

                    MessageBox.Show("Профиль успешно обновлён");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании профиля: {ex.Message}");
            }
        }


        // Добавляем этот метод в ваш класс
        private void RefreshProfilesList()
{
    try
    {
        using (var db = new AppDbContext())
        {
            var profiles = db.Profiles.ToList();
            ProfilesListBox.ItemsSource = profiles;
            
            // Сохраняем выбранный профиль (если он есть в обновленном списке)
            if (ProfilesListBox.SelectedItem != null)
            {
                var selectedId = ((Profile)ProfilesListBox.SelectedItem).Id;
                ProfilesListBox.SelectedItem = profiles.FirstOrDefault(p => p.Id == selectedId);
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при обновлении списка профилей: {ex.Message}");
    }
}

        // Обработчики для вкладки автоматических действий
        private void AutoActionsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedItem = e.Row.Item as AutoAction;
                dbManager.UpdateAutoAction(editedItem);
            }
        }

        // Обработчики для вкладки мониторинга
        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            // Логика начала мониторинга
            MessageBox.Show("Мониторинг запущен");
        }

        private void StopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            // Логика остановки мониторинга
            MessageBox.Show("Мониторинг остановлен");
        }

        private void ShowHistory_Click(object sender, RoutedEventArgs e)
        {
            if (FromDate.SelectedDate == null || ToDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите диапазон дат");
                return;
            }

            var monitoringData = dbManager.GetMonitoringData(FromDate.SelectedDate.Value, ToDate.SelectedDate.Value);
            // Здесь нужно обновить графики с полученными данными
            MessageBox.Show($"Загружено {monitoringData.Count} записей мониторинга");
        }

        // Обработчики для управления окном
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Сохранение настроек перед закрытием
            base.OnClosing(e);
        }
        private void TestRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshProfilesList();
            MessageBox.Show("Список профилей обновлён!");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
