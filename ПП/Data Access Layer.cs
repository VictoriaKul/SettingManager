using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ПП
{
    public class DatabaseManager
    {
        private readonly string connectionString;

        public DatabaseManager()
        {
            connectionString = GetConnectionString(); // Инициализируем строку подключения
        }

        public static string GetConnectionString()
        {
            // Проверяем, есть ли строка подключения в конфигурации
            var settings = ConfigurationManager.ConnectionStrings["MyAppDatabase"];

            if (settings == null)
                throw new Exception("Ошибка: строка подключения 'MyAppDatabase' не найдена в App.config!");

            if (string.IsNullOrEmpty(settings.ConnectionString))
                throw new Exception("Ошибка: строка подключения пустая!");

            return settings.ConnectionString;
        }
        // Методы для работы с автозагрузкой
        public ObservableCollection<StartupItem> GetStartupItems()
        {
            var items = new ObservableCollection<StartupItem>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM StartupItems", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StartupItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Path = reader.GetString(2),
                            Enabled = reader.GetBoolean(3),
                            Source = reader.GetString(4)
                        });
                    }
                }
            }

            return items;
        }
        public void UpdateProfile(Profile profile)
        {
            using (var db = new AppDbContext())
            {
                var existingProfile = db.Profiles.Find(profile.Id);
                if (existingProfile == null)
                {
                    throw new Exception("Профиль не найден в базе данных");
                }

                existingProfile.Name = profile.Name;
                existingProfile.Description = profile.Description;
                existingProfile.IsDefault = profile.IsDefault;

                db.SaveChanges();
            }
        }

        public void UpdateStartupItem(StartupItem item)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE StartupItems SET Name=@Name, Path=@Path, Enabled=@Enabled, Source=@Source WHERE Id=@Id",
                    connection);

                command.Parameters.AddWithValue("@Id", item.Id);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Path", item.Path);
                command.Parameters.AddWithValue("@Enabled", item.Enabled);
                command.Parameters.AddWithValue("@Source", item.Source);

                command.ExecuteNonQuery();
            }
        }

        // Методы для работы с профилями
        public ObservableCollection<Profile> GetProfiles()
        {
            var profiles = new ObservableCollection<Profile>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Profiles", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        profiles.Add(new Profile
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return profiles;
        }

        public void AddProfile(Profile profile)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO Profiles (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();",
                    connection);

                command.Parameters.AddWithValue("@Name", profile.Name);
                profile.Id = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void DeleteProfile(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Profiles WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        // Методы для работы с автоматическими действиями
        public ObservableCollection<AutoAction> GetAutoActions()
        {
            var actions = new ObservableCollection<AutoAction>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM AutoActions", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actions.Add(new AutoAction
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Condition = reader.GetString(2),
                            Action = reader.GetString(3),
                            IsEnabled = reader.GetBoolean(4)
                        });
                    }
                }
            }

            return actions;
        }

        public void UpdateAutoAction(AutoAction action)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE AutoActions SET Name=@Name, Condition=@Condition, Action=@Action, IsEnabled=@IsEnabled WHERE Id=@Id",
                    connection);

                command.Parameters.AddWithValue("@Id", action.Id);
                command.Parameters.AddWithValue("@Name", action.Name);
                command.Parameters.AddWithValue("@Condition", action.Condition);
                command.Parameters.AddWithValue("@Action", action.Action);
                command.Parameters.AddWithValue("@IsEnabled", action.IsEnabled);

                command.ExecuteNonQuery();
            }
        }

        // Методы для работы с мониторингом
        public ObservableCollection<MonitoringData> GetMonitoringData(DateTime fromDate, DateTime toDate)
        {
            var data = new ObservableCollection<MonitoringData>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT * FROM MonitoringData WHERE Date BETWEEN @FromDate AND @ToDate ORDER BY Date",
                    connection);

                command.Parameters.AddWithValue("@FromDate", fromDate);
                command.Parameters.AddWithValue("@ToDate", toDate);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new MonitoringData
                        {
                            Date = reader.GetDateTime(0),
                            CpuUsage = reader.GetDouble(1),
                            MemoryUsage = reader.GetDouble(2)
                        });
                    }
                }
            }

            return data;
        }
    }
}
