using Npgsql;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WinFormsApp1;

namespace UsersApp
{
    public partial class Page2 : Window
    {
        private readonly long currentUserId;
        private readonly DB db;

        public class UserItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public Page2(long userId)
        {
            InitializeComponent();
            this.currentUserId = userId;
            this.ResizeMode = ResizeMode.NoResize;
            db = new DB();
            LoadUsers();
        }

        private void LoadUsers()
        {
            List<UserItem> users = new List<UserItem>();

            using (var connection = db.GetConnection())
            {
                string sql = "SELECT id, name, email FROM users WHERE id != @currentUserId";
                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@currentUserId", currentUserId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserItem
                            {
                                Id = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            UsersListView.ItemsSource = users;
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is UserItem selectedUser)
            {
                this.Hide();
                Page1 chatPage = new Page1(currentUserId, selectedUser.Id);
                chatPage.Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для начала чата.");
            }
        }
    }
}
