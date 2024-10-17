using Npgsql;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.Data;
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
using WinFormsApp1;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace UsersApp
{
    /// <summary>
    /// Логика взаимодействия для WindowOne.xaml
    /// </summary>
    public partial class WindowOne : Window
    {
        private readonly long currentUserId;
        private readonly DB db;


        public WindowOne(long userId)
        {
            InitializeComponent();
            this.currentUserId = userId;
            this.ResizeMode = ResizeMode.NoResize;
        //    this.recipientId = recipientId;
            db = new DB();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            entry entryy = new entry();
            entryy.Show();
        }

        private void LoadHandler(object sender, RoutedEventArgs e)
        {
            if (currentUserId > 0)
            {
                using (var connection = db.GetConnection())
                {
                    using (var command = new NpgsqlCommand(
                        "SELECT name, email, surname, login, lastname, phone, date " +
                        "FROM users WHERE id = @userId", connection))
                    {
                        command.Parameters.AddWithValue("@userId", currentUserId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Отображаем информацию о пользователе
                                fistname.Text = $"Имя: {reader["name"]}";
                                Lastname.Text = $"Фамилия: {reader["surname"]}";
                                emaill.Text = $"Почта: {reader["email"]}";
                                Loginnn.Text = $"Логин: {reader["login"]}";
                                Namelasttt.Text = $"Отчество: {reader["lastname"]}"; // Изменено на "lastname"
                                phone.Text = $"Телефон: {reader["phone"]}";
                                date.Text = $"Дата рождения: {reader["date"]}"; // Убедитесь, что тип данных совместим
                                id.Text = "запись: " + currentUserId.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Пользователь не найден.");
                            }
                        }
                    }
                }
            }
        }

        //private void Назад(object sender, RoutedEventArgs e)
        //{
        //    this.Hide();
        //    Page1 entryy = new Page1(currentUserId, recipientId);
        //    entryy.Show();
        //}
    }
}
