using Npgsql; // Не забудьте добавить ссылку на Npgsql в проект
using System;
using System.Windows;
using System.Windows.Media;
using WinFormsApp1;

namespace UsersApp
{
    public partial class entry : Window
    {
        public long CurrentUserId { get; private set; }
        private readonly DB db;

        public entry()
        {
            InitializeComponent();
            db = new DB();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void ButtonEntry_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin1.Text.Trim();
            string password = PassBox1.Password;

            if (login.Length < 5)
            {
                TextBoxLogin1.ToolTip = "Это поле введено не корректно!";
                TextBoxLogin1.Background = Brushes.DarkRed;
                return;
            }

            TextBoxLogin1.ToolTip = "";
            TextBoxLogin1.Background = Brushes.Transparent;

            using (var connection = db.GetConnection())
            {
                using (var command = new NpgsqlCommand("SELECT id, login, pass FROM users WHERE login = @uL AND pass = @uP", connection))
                {
                    command.Parameters.AddWithValue("@uL", login);
                    command.Parameters.AddWithValue("@uP", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.CurrentUserId = reader.GetInt64(0);
                            this.Hide();
                            WindowOne page = new WindowOne(this.CurrentUserId);
                            page.Show();
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль.");
                        }
                    }
                }
            }

            TextBoxLogin1.Clear();
            PassBox1.Clear();
        }

        private void ButtonRegist_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow registr = new MainWindow();
            registr.Show();
        }
    }
}
