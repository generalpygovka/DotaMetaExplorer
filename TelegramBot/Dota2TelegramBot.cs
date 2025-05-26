using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net.Http.Json;
using System.Text;
using DotaMetaExplorer.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.SignalR;
namespace TelegramBot
{
    public class Dota2TelegramBot
    {
        private readonly HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://dotametaexplorer:8080") };
        TelegramBotClient botClient = new TelegramBotClient("7247243723:AAFs8m30615JIbYOse1fOW-hMEQhZfbU2Ok");
        CancellationToken cancellationToken = new CancellationToken();
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        private string? _lastPatchVersion = "7.38";
        
        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync,HandlerError,receiverOptions,cancellationToken);

            var botMe = await botClient.GetMe();
            Console.WriteLine($"Бот {botMe.Username} почав працювати");
            Console.ReadKey();
        }

        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Ошибка в Telegram бот API:\n{apiRequestException.ErrorCode}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(botClient, update.Message);
            }
        }

        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start" || message.Text == "Повернутись назад")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                new KeyboardButton[] { "🦸 Герої", "🧑‍💻 Гравці" },
                new KeyboardButton[] { "🏆 Команди", "⚔️ Останні Матчі" },
                new KeyboardButton[] { "📝 Останній патч", "🔔 Підписки" },
                new KeyboardButton[] { "🎬 GIF", "🎲 Випадковий Герой" },
                new KeyboardButton[]  { "🏆 Топ-10 гравців" }
                })
                {
                    ResizeKeyboard = true
                };

                await botClient.SendMessage(chatId: message.Chat.Id,"Оберіть поле",replyMarkup: replyKeyboard);
            }
            if (message.Text == "🦸 Герої")
            {
                await botClient.SendMessage(message.Chat.Id, "Введіть ім'я героя або його ID, наприклад: /hero axe або /hero 1");
                return;
            }
            if (message.Text == "🧑‍💻 Гравці")
            {
                await botClient.SendMessage(message.Chat.Id, "Введіть ID гравця /player <id> або використайте команду /players для списку.");
                return;
            }
            if (message.Text == "🏆 Команди")
            {
                await botClient.SendMessage(message.Chat.Id, "Введіть назву команди /team <name> або використайте команду /teams для списку.");
                return;
            }
            if (message.Text == "📝 Останній патч")
            {
                var response1 = await httpClient.GetAsync("api/Patch/GetLatestPatch");
                var response2 = await httpClient.GetAsync("api/Patch/GetLatestNotes");
                if (!response1.IsSuccessStatusCode)
                {
                    await botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response1.StatusCode}");
                    return;
                }
                var patch = await response1.Content.ReadFromJsonAsync<PatchList>();
                var patchNotes = await response2.Content.ReadFromJsonAsync<PatchNotes>();
                if (patch == null)
                {
                    await botClient.SendMessage(message.Chat.Id, "Немає даних про останній патч.");
                    return;
                }
                var sb = new StringBuilder();
                List<GeneralNoteSection> notes = patchNotes.GeneralNotes;

                var sb1 = new StringBuilder();
                foreach (var section in notes)
                {
                    if (!string.IsNullOrWhiteSpace(section.Title))
                        sb1.AppendLine($"*{section.Title.ToUpper()}*");

                    if (section.Generic != null && section.Generic.Count > 0)
                    {
                        foreach (var entry in section.Generic)
                        {
                            if (!string.IsNullOrWhiteSpace(entry.Note))
                                sb1.AppendLine($"- {entry.Note}");
                        }
                    }
                    sb1.AppendLine();
                }
                string result = sb1.ToString();

                sb.AppendLine($"Останній патч: {patchNotes.PatchName}");
                sb.AppendLine($"Головні зміни: \n{result}");
                const int telegramMessageLimit = 4096;
                string text = sb.ToString();

                if (text.Length <= telegramMessageLimit)
                {
                    await botClient.SendMessage(message.Chat.Id, text);
                }
                else
                {
                    for (int i = 0; i < text.Length; i += telegramMessageLimit)
                    {
                        var part = text.Substring(i, Math.Min(telegramMessageLimit, text.Length - i));
                        await botClient.SendMessage(message.Chat.Id, part);
                    }
                }
                return;
            }

            if (message.Text == "🔔 Підписки")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                new KeyboardButton[] { "➕ Додати підписку"},
                new KeyboardButton[] { "📋 Показати підписку", "🗑️ Видалити останню підписку" },
                new KeyboardButton[] { "Повернутись назад"}
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendMessage(message.Chat.Id,"Оберіть поле", replyMarkup: replyKeyboard);
            }
            if (message.Text == "➕ Додати підписку")
            {
                await botClient.SendMessage(message.Chat.Id, "Щоб підписатися на оновлення, використайте команду /subscribe <id героя> <id команди>" +
                    "Також якщо хочете підписатись на оновлення нового патчу то використайте тоді /subscribe <id героя> <id команди> <true>");
                return;
            }
            if (message.Text == "🎬 GIF")
            {
                await botClient.SendMessage(message.Chat.Id, "Введіть /gif <тег>, щоб знайти гіфку за темою. Наприклад: /gif dota2");
                return;
            }
            if (message.Text == "🎲 Випадковий Герой")
            {
                var response = await httpClient.GetAsync("api/Hero/GetRandomHero");
                var randomHero = await response.Content.ReadFromJsonAsync<Hero>();
                if (randomHero == null)
                {
                    var randomHeroAgain = await response.Content.ReadFromJsonAsync<Hero>();
                    await botClient.SendMessage(message.Chat.Id, $"Дані для {randomHeroAgain.LocalizedName}\n— Його атрибут: {randomHeroAgain.PrimaryAttr}\n— Його ID: {randomHeroAgain.Id}");
                }
                await botClient.SendMessage(message.Chat.Id, $"Дані для {randomHero.LocalizedName}\n— Його атрибут: {randomHero.PrimaryAttr}\n— Його ID: {randomHero.Id}");
                return;
            }
            if (message.Text == "⚔️ Останні Матчі")
            {
                var response = await httpClient.GetAsync("api/Match/GetRecentMatches");
                if (!response.IsSuccessStatusCode)
                {
                    await botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                var matches = await response.Content.ReadFromJsonAsync<List<ProMatches>>();
                if (matches == null || matches.Count == 0)
                {
                    await botClient.SendMessage(message.Chat.Id, "Немає даних про матчі.");
                    return;
                }
                matches = matches.Take(11).ToList();

                var sb = new StringBuilder();
                sb.AppendLine("Останні 10 матчів:");
                int i = 1;
                foreach (var match in matches)
                {
                    sb.AppendLine(new string('-', 20));
                    sb.AppendLine($"Матч №{i++}");
                    sb.AppendLine($"ID матчу: {match.MatchId}");
                    sb.AppendLine($"Radiant: {(string.IsNullOrEmpty(match.RadiantName) ? "Назва відсутня" : match.RadiantName)} vs " +
                        $"Dire: {(string.IsNullOrEmpty(match.DireName) ? "Назва відсутня" : match.DireName)}");
                    sb.AppendLine($"Переможець: {(match.RadiantWin ? "Radiant" : "Dire")}");
                    sb.AppendLine($"Тривалість: {TimeSpan.FromSeconds(match.Duration).ToString(@"hh\:mm\:ss")}");
                }
                await botClient.SendMessage(message.Chat.Id, sb.ToString());
                return;
            }
            if (message.Text == "🏆 Топ-10 гравців")
            {
                var response = await httpClient.GetAsync($"api/Player/GetLeaderboard");
                if (!response.IsSuccessStatusCode)
                {
                    await botClient.SendMessage(message.Chat.Id, $"Ошибка API: {(int)response.StatusCode}");
                    return;
                }
                var leaderboard = await response.Content.ReadFromJsonAsync<List<TopPlayerDto>>();
                if (leaderboard == null)
                {
                    await botClient.SendMessage(message.Chat.Id, "Немає даних у топі гравців");
                    return;
                }
                var sb = new StringBuilder();
                sb.AppendLine("Топ 10 игроков:");
                foreach (var leaderboards in leaderboard)
                {
                    sb.AppendLine(new string('-', 20));
                    sb.AppendLine($"\nНік: {leaderboards.PersonaName}");
                    sb.AppendLine($"Аккаунт айді: {leaderboards.AccountId}");
                    sb.AppendLine($"Ранг у світовій таблиці: {leaderboards.LeaderboardRank}");
                }
                await botClient.SendMessage(message.Chat.Id, sb.ToString());
                return;
            }
            if (message.Text!.StartsWith("/gif"))
            {
                var parts = message.Text.Split(' ', 2);
                if (parts.Length < 2)
                {
                    await botClient.SendMessage(message.Chat.Id, "Використайте: /gif <тег>");
                    return;
                }
                var gifName = parts[1].Trim();
                var response = await httpClient.GetAsync($"api/Gif/GetByTag?tag={gifName}");
                if (!response.IsSuccessStatusCode)
                {
                    await botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                var gif = await response.Content.ReadFromJsonAsync<Giphy.RandomGiphy>();

                if (string.IsNullOrEmpty(gif?.Data?.Images?.FixedHeight?.Url))
                {
                    await botClient.SendMessage(message.Chat.Id, "Гіфка за цим тегом не знайдена.");
                    return;
                }
                await botClient.SendAnimation(message.Chat.Id, gif?.Data?.Images?.FixedHeight?.Url!, caption: $"Гіфка за тегом: {gifName}");
                return;
            }


            //if (message.Text == "/inline")
            //{
            //    InlineKeyboardMarkup keyboardMarkup = new
            //    (
            //        new[]
            //        {
            //        new[]
            //        {
            //            InlineKeyboardButton.WithCallbackData("Погода в Києві", "WeatherKyiv"),
            //            InlineKeyboardButton.WithCallbackData("Погода у Львові", "WeatherLviv")
            //        }
            //        }
            //    );
            //    await botClient.SendMessage(message.Chat.Id, "Виберіть місто:", replyMarkup: keyboardMarkup);
            //    return;
            //}
            //if (message.Text == "/keyboard")
            //{
            //    ReplyKeyboardMarkup replyKeyboardMarkup = 
            //    new
            //    (
            //    new[]
            //        {
            //        new KeyboardButton[] { "Hello" , "Buy" },
            //        new KeyboardButton [] { "Hello Denis", "Hello Oleg"}
            //        }
            //    )
            //    {
            //        ResizeKeyboard = true
            //    };
            //    await botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: replyKeyboardMarkup);
            //    return;
            //}
            //if (message.Text == "Hello" || message.Text == "Hello Oleg" || message.Text == "Hello Denis")
            //{
            //    await botClient.SendMessage(message.Chat.Id, "І тобі привіт");
            //}
            //if (message.Text.StartsWith("/subscribe_hero"))
            //{
            //    var parts = message.Text.Split(' ', 2);
            //    if (parts.Length < 2)
            //    {
            //        await botClient.SendMessage(message.Chat.Id, "Используйте: /subscribe_hero <имя_героя>");
            //        return;
            //    }
            //    var hero = parts[1].Trim();
            //    Subscribe subscribe = new Subscribe { ChatId = message.Chat.Id, FavouriteHero = hero };
            //    var response = await httpClient.PostAsJsonAsync("api/Subscribe/Subscribe", subscribe);
            //    if (response.IsSuccessStatusCode)
            //        await botClient.SendMessage(message.Chat.Id, $"Вы подписались на героя: {hero}");
            //    else
            //        await botClient.SendMessage(message.Chat.Id, "Ошибка при подписке.");
            //    return;
            //}
            //if (message.Text.StartsWith("/subscribe_team"))
            //{
            //    var parts = message.Text.Split(' ', 2);
            //    if (parts.Length < 2)
            //    {
            //        await botClient.SendMessage(message.Chat.Id, "Используйте: /subscribe_team <имя_команды>");
            //        return;
            //    }
            //    var team = parts[1].Trim();
            //    var subscribe = new Subscribe { ChatId = message.Chat.Id, FavouriteTeam = team };
            //    var response = await httpClient.PostAsJsonAsync("api/Subscribe/Subscribe", subscribe);
            //    if (response.IsSuccessStatusCode)
            //        await botClient.SendMessage(message.Chat.Id, $"Вы подписались на команду: {team}");
            //    else
            //        await botClient.SendMessage(message.Chat.Id, "Ошибка при подписке.");
            //    return;
            //}
            if (message.Text.StartsWith("/subscribe"))
            {
                var parts = message.Text.Split(' ', 4);
                if (parts.Length < 3)
                {
                    await botClient.SendMessage(message.Chat.Id, "Використайте: /subscribe <id_героя> <id_команди>");
                    return;
                }
                if (!int.TryParse(parts[1], out int heroId) || !int.TryParse(parts[2], out int teamId))
                {
                    await botClient.SendMessage(message.Chat.Id, "ID героя та команди мають бути числами.");
                    return;
                }
                var patch = parts.Length > 3 && bool.TryParse(parts[3].Trim(), out bool isSubscribeForPatch) ? isSubscribeForPatch : false;
                var subscribe = new Subscribe
                {
                    ChatId = (int)message.Chat.Id,
                    FavouriteHeroId = heroId,
                    FavouriteTeamId = teamId,
                    IsSubscribeForPatch = patch
                };
                var response = await httpClient.PostAsJsonAsync("api/Subscribe/Subscribe", subscribe);
                if (response.IsSuccessStatusCode && patch == false)
                    await botClient.SendMessage(message.Chat.Id, $"Ви підписалися на героя з ID: {heroId} та команду з ID: {teamId}");
                else if (response.IsSuccessStatusCode && patch == true)
                    await botClient.SendMessage(message.Chat.Id, $"Ви підписалися на героя з ID: {heroId} та команду з ID: {teamId}, а також на оновлення патчу");
                else
                    await botClient.SendMessage(message.Chat.Id, "Помилка при підписці.");
                return;
            }
            if (message.Text == "📋 Показати підписку")
            {
                var response = await httpClient.GetAsync($"api/Subscribe/GetById?id={message.Chat.Id}");
                if (!response.IsSuccessStatusCode)
                {
                    await botClient.SendMessage(message.Chat.Id, "Помилка при отриманні підписок.");
                    return;
                }
                var subs = await response.Content.ReadFromJsonAsync<List<Subscribe>>();
                if (subs == null)
                {
                    await botClient.SendMessage(message.Chat.Id, "У вас немає підписок.");
                    return;
                }
                var sb = new StringBuilder();
                sb.AppendLine("Ваши подписки:");
                foreach (var sub in subs)
                {
                    sb.AppendLine($"Герой ID: {sub.FavouriteHeroId}, Команда ID: {sub.FavouriteTeamId}");
                }
                var subscribedForPatch = subs.Where(x => x.IsSubscribeForPatch != null);
                await botClient.SendMessage(message.Chat.Id, sb.ToString() + $"чи підписані на патч {subscribedForPatch}");
                return;
            }
            if (message.Text == "🗑️ Видалити останню підписку")
            {
                var response = await httpClient.DeleteAsync($"api/Subscribe/DeleteSubscribe?id={message.Chat.Id}");
                if (response.IsSuccessStatusCode)
                {
                    await botClient.SendMessage(message.Chat.Id, "Ваша остання підписка видалена.");
                }
                else
                {
                    await botClient.SendMessage(message.Chat.Id, "Помилка при видаленні підписки або у вас немає підписок.");
                }
                return;
            }

            if (message.Text!.StartsWith("/team"))
            {
                if (message.Text == "/teams")
                {
                    var response = await httpClient.GetAsync("api/Team/GetAllTeams");
                    if (!response.IsSuccessStatusCode)
                    {
                        await botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                        return;
                    }
                    var teams = await response.Content.ReadFromJsonAsync<List<Team>>();
                    if (teams == null || teams.Count == 0)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Немає даних про команди.");
                        return;
                    }
                    teams = teams.Take(50).ToList();
                    var sb = new StringBuilder();
                    sb.AppendLine("Список перших 50 команд:");
                    int i = 1;
                    foreach (var team in teams)
                    {
                        sb.AppendLine(new string('-', 20));
                        sb.AppendLine($"{i++}.");
                        sb.AppendLine($"Назва: {team.Name}");
                        sb.AppendLine($"ID: {team.TeamId}");
                        sb.AppendLine($"Тег: {team.Tag}");
                        sb.AppendLine($"Рейтинг: {team.Rating}");
                        sb.AppendLine($"Перемоги: {team.Wins}");
                        sb.AppendLine($"Поразки: {team.Losses}");
                    }

                    const int telegramMessageLimit = 4096;
                    var result = sb.ToString();
                    for (int j = 0; j < result.Length; j += telegramMessageLimit)
                    {
                        var part = result.Substring(j, Math.Min(telegramMessageLimit, result.Length - j));
                        await botClient.SendMessage(message.Chat.Id, part);
                    }
                    return;
                }
                else
                {
                    var parts = message.Text.Split(' ', 2);
                    if (parts.Length < 2)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Використайте: /team <id>");
                        return;
                    }
                    var teamName = parts[1].Trim();
                    var response = await httpClient.GetAsync($"api/Team/GetTeamByName?name={teamName}");
                    if (!response.IsSuccessStatusCode)
                    {
                        await botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                        return;
                    }
                    var team = await response.Content.ReadFromJsonAsync<Team>();
                    if (team == null)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Команду з такою назвою не знайдено.");
                        return;
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("Інформація про команду:");
                    sb.AppendLine($"Назва: {team.Name}");
                    sb.AppendLine($"ID: {team.TeamId}");
                    sb.AppendLine($"Тег: {team.Tag}");
                    sb.AppendLine($"Рейтинг: {team.Rating}");
                    sb.AppendLine($"Перемоги: {team.Wins}");
                    sb.AppendLine($"Поразки: {team.Losses}");

                    await botClient.SendMessage(message.Chat.Id, sb.ToString());
                    return;
                }
            }
            if (message.Text.StartsWith("/hero"))
            {
                var parts = message.Text.Split(' ', 2);
                if (parts.Length < 2)
                {
                    await botClient.SendMessage(message.Chat.Id, "Використайте: /hero <heroName> або /hero <id>");
                    return;
                }

                var heroInput = parts[1].Trim();
                if (int.TryParse(heroInput, out int heroId))
                {
                    var resp = await httpClient.GetAsync($"api/Hero/GetByIdHero?id={heroId}");
                    var meta = await resp.Content.ReadFromJsonAsync<Hero>();
                    if (meta == null)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Героя з таким ID не знайдено.");
                        return;
                    }
                    Console.WriteLine($"/hero {heroInput}");
                    await botClient.SendMessage(
                        message.Chat.Id,
                        $"Дані для {meta.LocalizedName}\n— Його атрибут: {meta.PrimaryAttr}"
                    );
                }
                else
                {
                    Console.WriteLine($"/hero {heroInput}");
                    var resp = await httpClient.GetAsync($"api/Hero/GetByName?name={heroInput.ToLower()}");
                    var meta = await resp.Content.ReadFromJsonAsync<Hero>();
                    if (meta == null)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Героя з таким ім'ям не знайдено.");
                        return;
                    }
                    await botClient.SendMessage(
                        message.Chat.Id,
                        $"Дані для {meta.LocalizedName}\n— Його атрибут: {meta.PrimaryAttr}\n— Його ID: {meta.Id}"
                    );
                    return;
                }
            }
            if (message.Text.StartsWith("/player"))
            {
                if (message.Text == "/players")
                {
                    var response = await httpClient.GetAsync("api/Player/GetProPlayers");
                    if (!response.IsSuccessStatusCode)
                    {
                        await botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                        return;
                    }
                    var players = await response.Content.ReadFromJsonAsync<List<ProPlayer>>();
                    if (players == null || players.Count == 0)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Немає даних про гравців.");
                        return;
                    }
                    if (players.Count > 50)
                    {
                        players = players.Take(51).ToList();
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("Список перших 50 гравців:");
                    int i = 0;
                    foreach (ProPlayer player in players)
                    {
                        
                        sb.AppendLine(new string('-', 20));
                        sb.AppendLine($"{i++}.");
                        sb.AppendLine($"Нік: {player.Name}");
                        sb.AppendLine($"ID: {player.AccountId}");
                        sb.AppendLine($"Команда: {player.TeamName}");
                    }
                    const int telegramMessageLimit = 4096;
                    var result = sb.ToString();
                    for (int j = 0; j < result.Length; j += telegramMessageLimit)
                    {
                        var part = result.Substring(j, Math.Min(telegramMessageLimit, result.Length - j));
                        await botClient.SendMessage(message.Chat.Id, part);
                    }
                    await botClient.SendMessage(message.Chat.Id, result);
                    return;
                }
                else
                {
                    var parts = message.Text.Split(' ', 2);
                    if (parts.Length < 2)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Використайте: /player <id>");
                        return;
                    }

                    var playerId = parts[1];
                    var response = await httpClient.GetAsync($"api/Player/GetPlayerById?id={playerId}");
                    if (!response.IsSuccessStatusCode)
                    {
                        await botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                        return;
                    }
                    var players = await response.Content.ReadFromJsonAsync<Player>();
                    if (players == null)
                    {
                        await botClient.SendMessage(message.Chat.Id, "Немає даних про гравця.");
                        return;
                    }
                    var sb = new StringBuilder();
                    sb.AppendLine($"Інформація про гравця:");
                    sb.AppendLine($"Нік: {players.Profile.Name}");
                    sb.AppendLine($"ID: {players.Profile.AccountId}");
                    sb.AppendLine($"Ранг у світі: {players.LeaderboardRank}");
                    sb.AppendLine($"Країна: {players.Profile.LocCountryCode}");
                    sb.AppendLine($"Останній вхід: {players.Profile.LastLogin}");

                    await botClient.SendPhoto(message.Chat.Id,players.Profile.Avatar!,caption: sb.ToString());
                    return;
                }
            }
        }
        public class TopPlayerDto
        {
            public string? PersonaName { get; set; }
            public int AccountId { get; set; }
            public int? LeaderboardRank { get; set; }
        }
        public class LastPatch
        {
            [JsonPropertyName("latest_patch")]
            public string? LatestPatch { get; set; }
        }
        public async Task CheckForNewPatchAsync()
        {
            while (true)
            {
                try
                {
                    var response = await httpClient.GetAsync("api/Patch/GetLatestPatch");
                    if (response.IsSuccessStatusCode)
                    {
                        var patch = await response.Content.ReadFromJsonAsync<LastPatch>();
                        if (patch != null && patch.LatestPatch != _lastPatchVersion)
                        {
                            _lastPatchVersion = patch.LatestPatch;

                            var subsResponse = await httpClient.GetAsync("api/Subscribe/GetAll");
                            var noteResponse = await httpClient.GetAsync("api/Patch/GetLatestNotes");
                            if (subsResponse.IsSuccessStatusCode)
                            {
                                var subscribe = await subsResponse.Content.ReadFromJsonAsync<List<Subscribe>>();

                                var heroNotes = await noteResponse.Content.ReadFromJsonAsync<PatchNotes>();

                                List<HeroSection> heroes = heroNotes!.Heroes!;
                                var grouped = subscribe!.GroupBy(s => s.ChatId);
                                var subscribedForPatch = new List<int>();
                                foreach (var group in grouped)
                                {
                                    int chatId = group.Key;
                                    if (!subscribedForPatch.Contains(chatId))
                                    {
                                        await botClient.SendMessage(chatId, $"Вийшов новий патч: {patch.LatestPatch}!");
                                        subscribedForPatch.Add(chatId);
                                    }
                                    
                                    foreach (var sub in group)
                                    {
                                        var heroSection = heroes.FirstOrDefault(x => x.HeroId == sub.FavouriteHeroId);
                                        if (heroSection != null)
                                        {
                                            const int telegramMessageLimit = 4096;
                                            var result = FormatHeroSection(heroSection);
                                            for (int j = 0; j < result.Length; j += telegramMessageLimit)
                                            {
                                                var part = result.Substring(j, Math.Min(telegramMessageLimit, result.Length - j));
                                                await botClient.SendMessage(chatId, part);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при перевірці патча: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
        public string FormatHeroSection(HeroSection heroSection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Герой ID: {heroSection.HeroId}");

            if (heroSection.HeroNotes != null && heroSection.HeroNotes.Count > 0)
            {
                sb.AppendLine("Заметки героя:");
                foreach (var note in heroSection.HeroNotes)
                    sb.AppendLine($"- {note.Note}");
            }
  
            if (heroSection.TalentNotes != null && heroSection.TalentNotes.Count > 0)
            {
                sb.AppendLine("Таланты:");
                foreach (var note in heroSection.TalentNotes)
                    sb.AppendLine($"- {note.Note}");
            }

            if (heroSection.Abilities != null && heroSection.Abilities.Count > 0)
            {
                sb.AppendLine("Изменения способностей:");
                foreach (var ability in heroSection.Abilities)
                {
                    sb.AppendLine($"  {ability.Title}:");
                    if (ability.AbilityNotes != null)
                        foreach (var note in ability.AbilityNotes)
                            sb.AppendLine($"    - {note.Note}");
                }
            }

            if (heroSection.Subsections != null && heroSection.Subsections.Count > 0)
            {
                foreach (var sub in heroSection.Subsections)
                {
                    sb.AppendLine($"Врождёнка: {sub.Title}");
                    if (sub.GeneralNotes != null)
                        foreach (var note in sub.GeneralNotes)
                            sb.AppendLine($"  - {note.Note}");
                    if (sub.Abilities != null)
                        foreach (var ability in sub.Abilities)
                        {
                            sb.AppendLine($"  {ability.Title}:");
                            if (ability.AbilityNotes != null)
                                foreach (var note in ability.AbilityNotes)
                                    sb.AppendLine($"    - {note.Note}");
                        }
                    if (sub.TalentNotes != null)
                        foreach (var note in sub.TalentNotes)
                            sb.AppendLine($"  - {note.Note}");
                }
            }

            return sb.ToString();
        }

    }
}

