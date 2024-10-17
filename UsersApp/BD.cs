using System;
using System.Data;
using Npgsql; // Не забудьте добавить ссылку на Npgsql в проект

namespace WinFormsApp1
{
    internal class DB
    {
        // Замените строку подключения на свою
        private static readonly string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=00000000;Database=user;";
        private NpgsqlConnection connection;

        public NpgsqlConnection GetConnection()
        {
            if (connection == null || connection.State == ConnectionState.Closed)
            {
                try
                {
                    connection = new NpgsqlConnection(connectionString);
                    connection.Open();
                    Console.WriteLine("Соединение с базой данных успешно установлено.");
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
                    throw new Exception("Ошибка при открытии соединения с базой данных", ex);
                }
            }
            return connection;
        }

        public void CloseConnect()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Соединение с базой данных закрыто.");
            }
        }
    }
}
