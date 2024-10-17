using Npgsql;
using System.Windows;
using WinFormsApp1;

namespace UsersApp
{
    public partial class Page3 : Window
    {
        private readonly long currentUserId;
        private readonly DB db;

        public Page3(long userId)
        {
            InitializeComponent();
            this.currentUserId = userId;
            this.db = new DB();
            LoadSettings();
        }

        private void LoadSettings()
        {
            using (var connection = db.GetConnection())
            {
                // Загружаем информацию о профиле пользователя
                string sql = "SELECT name, email FROM users WHERE id = @userId";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userId", currentUserId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ProfileNameTextBox.Text = reader.GetString(0);
                            ProfileEmailTextBox.Text = reader.GetString(1);
                        }
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = db.GetConnection())
            {
                string sql = "UPDATE users SET name = @name, email = @email WHERE id = @userId";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", ProfileNameTextBox.Text);
                    command.Parameters.AddWithValue("@email", ProfileEmailTextBox.Text);
                    command.Parameters.AddWithValue("@userId", currentUserId);
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Настройки сохранены успешно.");
            this.Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
