using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Npgsql; // Не забудьте добавить ссылку на Npgsql в проект
using WinFormsApp1;

namespace UsersApp
{
    public class ChatManager
    {
        private readonly DB db;

        public ChatManager()
        {
            db = new DB();
        }

        public bool SendMessage(string message, string senderId, string recipientId)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            using (var connection = db.GetConnection())
            using (var command = new NpgsqlCommand(
                "INSERT INTO messages (user_id, recipient_id, message, created_at) VALUES (@userId, @recipientId, @message, NOW())",
                connection))
            {
                command.Parameters.AddWithValue("@userId", long.Parse(senderId));
                command.Parameters.AddWithValue("@recipientId", long.Parse(recipientId));
                command.Parameters.AddWithValue("@message", message);
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<Message> GetMessages(long userId, long recipientId)
        {
            var messages = new List<Message>();

            using (var connection = db.GetConnection())
            {
                try
                {
                    string sql = @"SELECT m.user_id, m.message, m.created_at 
                                   FROM messages m
                                   WHERE (m.user_id = @userId AND m.recipient_id = @recipientId)
                                   OR (m.user_id = @recipientId AND m.recipient_id = @userId)
                                   ORDER BY m.created_at DESC 
                                   LIMIT 50";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@recipientId", recipientId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var message = new Message
                                {
                                    UserId = reader.GetInt64(0).ToString(),
                                    Text = reader.GetString(1),
                                    CreatedAt = reader.GetDateTime(2)
                                };
                                messages.Add(message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occurred in GetMessages: {ex.Message}");
                    MessageBox.Show($"Stack trace: {ex.StackTrace}");
                }
            }

            return messages;
        }

        public class Message
        {
            public string UserId { get; set; }
            public string Text { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
