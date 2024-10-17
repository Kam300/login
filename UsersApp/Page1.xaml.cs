using Npgsql; // не забудьте добавить ссылку на Npgsql в проект
using System;
using System.Windows;
using System.Windows.Threading;
using WinFormsApp1;

namespace UsersApp
{
    public partial class Page1 : Window
    {
        private readonly long currentUserId;
        private long recipientId;
        private readonly ChatManager chatManager;
        private DispatcherTimer timer;
        private readonly DB db;
        private bool isWindowClosed = false;
        private bool hasShownNewMessageNotification = false;

        public Page1(long currentUserId, long recipientId)
        {
            InitializeComponent();
            this.currentUserId = currentUserId;
            this.recipientId = recipientId;
            chatManager = new ChatManager();
            UpdateChatArea();
            this.db = new DB();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5); // Проверять каждые 5 секунд
            timer.Tick += Timer_Tick;
            timer.Start();
            Closed += Page1_Closed;
        }

        private void Page1_Closed(object sender, EventArgs e)
        {
            // Останавливаем таймер
            isWindowClosed = true;
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Проверяем наличие новых сообщений
            var newMessages = chatManager.GetMessages(currentUserId, recipientId);
            if (newMessages.Count > 0)
            {
                // Показываем уведомление о новых сообщениях
                if (!hasShownNewMessageNotification)
                {
                    MessageBox.Show($"Новое(ые) сообщение(я) от собеседника!");
                    hasShownNewMessageNotification = true;
                }

                // Обновляем область чата
                UpdateChatArea();
            }
            else
            {
                // Сбрасываем флаг, чтобы уведомление о новых сообщениях снова показывалось
                hasShownNewMessageNotification = false;
            }
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageInput.Text))
            {
                try
                {
                    bool messageSent = chatManager.SendMessage(MessageInput.Text, currentUserId.ToString(), recipientId.ToString());
                    if (messageSent)
                    {
                        MessageInput.Clear();
                        UpdateChatArea();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось отправить сообщение. Пожалуйста, попробуйте еще раз.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке сообщения: {ex.Message}");
                }
            }
        }

        private void UpdateChatArea()
        {
            ChatArea.Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatArea.Items.Clear();
                var messages = chatManager.GetMessages(this.currentUserId, this.recipientId);

                if (messages.Count == 0)
                {
                    ChatArea.Items.Add("Сообщений не найдено.");
                    return;
                }

                foreach (var message in messages.OrderBy(m => m.CreatedAt))
                {
                    string messageText;
                    if (message.UserId == currentUserId.ToString())
                    {
                        messageText = $"Вы: {message.Text} ({message.CreatedAt.ToLocalTime()})";
                    }
                    else
                    {
                        // Получаем имя собеседника из базы данных
                        string recipientName = GetRecipientName(long.Parse(message.UserId));
                        messageText = $"{recipientName}: {message.Text} ({message.CreatedAt.ToLocalTime()})";
                    }

                    ChatArea.Items.Add(messageText);
                }
            }));
        }

        private string GetRecipientName(long userId)
        {
            using (var connection = db.GetConnection())
            {
                string sql = "SELECT name FROM users WHERE id = @userId";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }
            return "Undefined";
        }

        private void ToggleLeftPanel(object sender, RoutedEventArgs e)
        {
            if (LeftPanel.Visibility == Visibility.Collapsed)
            {
                LeftPanel.Visibility = Visibility.Visible;
            }
            else
            {
                LeftPanel.Visibility = Visibility.Collapsed;
            }
        }

        //private void ProphilClick(object sender, RoutedEventArgs e)
        //{
        //    this.Hide();
        //    WindowOne one = new WindowOne(this.currentUserId, this.recipientId);
        //    one.Show();
        //}

        private void Home(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Page2 page2 = new Page2(recipientId);
            page2.Show();
        }

        //private void Account(object sender, RoutedEventArgs e)
        //{
        //    this.Hide();
        //    WindowOne one = new WindowOne(this.currentUserId, this.recipientId);
        //    one.Show();
        //}

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Hide();
            entry Entry = new entry();
            Entry.Show();
        }

        private void ShowPage3Button_Click(object sender, RoutedEventArgs e)
        {
            // Отображает страницу Page3
            Page3 page3 = new Page3(this.currentUserId);
            page3.Show();
        }
    }
}
