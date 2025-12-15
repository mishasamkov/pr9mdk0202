using TaskManagerTelegramBot_Classes;
using Telegram.Bot;
using TaskManagerTelegramBot.Classes;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskManagerTelegramBot
{
    public class Worker : BackgroundService
    {
        readonly string Token = "8597751564:AAHZmAMcwdV-2FzwipaOGm_ymxXvcUBkNgs";
        TelegramBotClient TelegramBotClient;
        List<Users> Users = new List<Users>();
        Timer Timer;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        static List<string> Messages = new List<string>()
        {
            "Здравствуйте!" +
            "\nРады приветствовать вас в Telegram-боте «Напоминатор»!" +
            "\nНаш бот создан для того, чтобы напоминать вам о важных событиях и мероприятиях. С ним вы точно не пропустите ничего важного! " +
            "\nНе забудьте добавить бота в список своих контактов и настроить уведомления. Тогда вы всегда будете в курсе событий! ",

            "Укажите дату и время напоминания в следующем формате:" +
            "\n<i><b>12:51 26.04.2025</b>" +
            "\nНапомни о том что я хотел сходить в магазин.</i>" +
            "\n\nДля повторяющихся задач используйте:" +
            "\n<i><b>* 21:00 ср,вс</b>" +
            "\nПолить цветы.</i>" +
            "\n\nПоддерживаются дни: пн, вт, ср, чт, пт, сб, вс",

            "Кажется, что-то не получилось." +
            "Укажите дату и время напоминания в следующем формате:" +
            "\n<i><b>12:51 26.04.2025</b>" +
            "\nНапомни о том что я хотел сходить в магазин.</i>" +
            "\n\nИли для повторяющихся задач:" +
            "\n<i><b>* 21:00 ср,вс</b>" +
            "\nЗадача.</i>",

            "Задачи пользователя не найдены.",
            "Событие удалено.",
            "Все события удалены."
        };
        public static InlineKeyboardMarkup DeleteEvent(string Message)
        {
            List<InlineKeyboardButton> inlineKeyboards = new List<InlineKeyboardButton>();
            inlineKeyboards.Add(new InlineKeyboardButton("Удалить", Message));

            return new InlineKeyboardMarkup(inlineKeyboards);
        }
        public async Task SendMessageAsync(long chatId, int typeMessage)
        {
            if (typeMessage != 3)
            {
                await TelegramBotClient.SendMessage(
                    chatId,
                    Messages[typeMessage],
                    ParseMode.Html,
                    replyMarkup: GetButtons());
            }
            else if (typeMessage == 3)
            {
                await TelegramBotClient.SendMessage(
                    chatId,
                    $"Указанное вами время и дата не могут быть установлены; " +
                    $"потому что сейчас уже: {DateTime.Now.ToString("HH.mm dd.MM.yyyy")}");
            }
        }
        public async Task CommandAsync(long chatId, string command)
        {
            if (command.ToLower() == "/start") await SendMessageAsync(chatId, 0);
            else if (command.ToLower() == "/create_task") await SendMessageAsync(chatId, 1);
            else if (command.ToLower() == "/list_tasks")
            {
                Users User = Users.Find(x => x.IdUser == chatId);
                if (User == null) await SendMessageAsync(chatId, 4);
                else if (User.Events.Count == 0) await SendMessageAsync(chatId, 4);
                else
                {
                    foreach (Events Event in User.Events)
                    {
                        string eventType = Event.IsRecurring ? "?? Повторяющаяся: " : "?? Одноразовая: ";
                        string eventInfo = Event.IsRecurring ?
                            $"Каждый {GetDaysString(Event.RecurringDays)} в {Event.Time:t}" :
                            $"Напоминание: {Event.Time.ToString("HH:mm dd.MM.yyyy")}";

                        await TelegramBotClient.SendMessage(
                            chatId,
                            $"{eventType}{eventInfo}" +
                            $"\nСообщение: {Event.Message}",
                            replyMarkup: DeleteEvent(Event.Message)
                            );
                    }
                }
            }
        }
        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message)
                await GetMessagesAsync(update.Message);
            else if (update.Type == UpdateType.CallbackQuery)
            {
                CallbackQuery query = update.CallbackQuery;
                Users User = Users.Find(x => x.IdUser == query.Message.Chat.Id);
                Events Event = User.Events.Find(x => x.Message == query.Data);
                User.Events.Remove(Event);
                DatabaseHelper.DeleteEvent(query.Message.Chat.Id, query.Data);

                await SendMessageAsync(query.Message.Chat.Id, 5);
            }
        }
        private string GetDaysString(List<DayOfWeek> days)
        {
            return string.Join(",", days.Select(d => DayOfWeekReverseMap[d]));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
