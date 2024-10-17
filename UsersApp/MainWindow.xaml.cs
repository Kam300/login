using Npgsql; 
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinFormsApp1;

namespace UsersApp
{
    public partial class MainWindow : Window
    {
        private readonly DB _db;

        public MainWindow()
        {
            InitializeComponent();
            _db = new DB();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentDate = DateTime.Now;

            string login = TextBoxLogin.Text.Trim();
            string pass = PassBox.Password.Trim();
            string pass2 = PassBox2.Password.Trim();
            string email = TextBoxEmail.Text.Trim();
            string firstName = fistname.Text.Trim();
            string lastName = lastname.Text.Trim();
            string lastName2 = lastlastname.Text.Trim();
            string phone = TextBoxPhone.Text.Trim();
            string date = DatePickerDate.SelectedDate?.ToString("yyyy-MM-dd");
            bool Check;

            bool isValid = ValidateInputs(login, pass, pass2, email, firstName, lastName, lastName2, phone);

            if (isValid)
            {
                if (CheckUser())
                {
                    MessageBox.Show("Логин уже существует. Пожалуйста, выберите другой.");
                    return;
                }

                using (var connection = _db.GetConnection())
                {
                    string sql = @"INSERT INTO users (login, pass, name, surname, lastname, phone, date, email) 
                                   VALUES (@login, @pass, @name, @surname, @lastname, @phone, @date, @email)";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@pass", pass); 
                        command.Parameters.AddWithValue("@name", firstName);
                        command.Parameters.AddWithValue("@surname", lastName);
                        command.Parameters.AddWithValue("@lastname", lastName2);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@date", DatePickerDate.SelectedDate.HasValue ? DatePickerDate.SelectedDate.Value : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@email", email);

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected == 1)
                            {
                                MessageBox.Show("Аккаунт создан!");
                            }
                            else
                            {
                                MessageBox.Show("Аккаунт не создан.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при создании аккаунта: {ex.Message}");
                            Console.WriteLine($"Ошибка: {ex}");
                        }
                    }
                }
            }
        }

        private bool ValidateInputs(string login, string pass, string pass2, string email, string firstName, string lastName, string lastName2, string phone)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Логин не может быть пустым.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Пароль не может быть пустым.");
                return false;
            }
            if (pass != pass2)
            {
                MessageBox.Show("Пароли не совпадают.");
                return false;
            }
            if (pass.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать как минимум 6 символов.");
                return false;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный адрес электронной почты.");
                return false;
            }
            if (firstName.Length > 50 || lastName.Length > 50 || lastName2.Length > 50)
            {
                MessageBox.Show("Слишком длинные имена.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhone(phone))
            {
                MessageBox.Show("Введите корректный номер телефона.");
                return false;
            }
            if(Check.IsChecked == false)
            {
                MessageBox.Show("Галочка не омечена");
                return false;
            }
            return true;
        }

        private bool IsValidEmail(string email)
        {
            // Простейшая проверка формата email
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        private bool IsValidPhone(string phone)
        {
            // Простейшая проверка формата телефона (например, +1234567890 или 1234567890)
            var phoneRegex = new Regex(@"^\+?\d{10,15}$");
            return phoneRegex.IsMatch(phone);
        }

        public bool CheckUser()
        {
            DataTable table = new DataTable();

            using (var connection = _db.GetConnection())
            {
                using (var command = new NpgsqlCommand("SELECT * FROM users WHERE login = @uL", connection))
                {
                    command.Parameters.AddWithValue("@uL", TextBoxLogin.Text);

                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }

            if (table.Rows.Count > 0)
            {
                TextBoxLogin.BorderBrush = Brushes.Red;
                return true; // Логин уже существует
            }

            TextBoxLogin.BorderBrush = Brushes.LightGray; // Сбрасываем цвет, если логин уникален
            return false; // Логин уникален
        }

        private void ButtonEntrance_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            entry Entry = new entry();
            Entry.Show();
        }

        private void DatePickerDate_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DatePickerDate.SelectedDate.HasValue)
            {
                DateTime selectedDate = DatePickerDate.SelectedDate.Value;
                MessageBox.Show($"Выбранная дата: {selectedDate.ToString("d")}");
            }
        }
    }
}
