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
