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

namespace TelegramBot
{
    public class Dota2TelegramBot
    {
        private readonly HttpClient _httpClient = new HttpClient() { BaseAddress = new Uri("https://localhost:7030/") };
        private readonly TelegramBotClient _botClient = new("7247243723:AAFs8m30615JIbYOse1fOW-hMEQhZfbU2Ok");
        private readonly CancellationToken _cancellationToken = new();
        private readonly ReceiverOptions _receiverOptions = new() { AllowedUpdates = { } };
        private string? _lastPatchVersion = "7.38";
        private const int TelegramMessageLimit = 4096;

        public async Task Start()
        {
            _botClient.StartReceiving(HandleUpdateAsync, HandleError, _receiverOptions, _cancellationToken);
            var botMe = await _botClient.GetMe();
            Console.WriteLine($"Бот {botMe.Username} почав працювати");
            Console.ReadKey();
        }

        private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Ошибка в Telegram бот API:\n{apiRequestException.ErrorCode}",
                _ => exception.ToString()
            };
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessageAsync(update.Message);
            }
        }

        private async Task HandleMessageAsync(Message message)
        {
            switch (message.Text)
            {
                case "/start":
                    await MainMenu(message.Chat.Id);
                    break;
                case "Повернутись назад":
                    await MainMenu(message.Chat.Id);
                    break;
                case "🦸 Герої":
                    await _botClient.SendMessage(message.Chat.Id, "Введіть ID героя або його назву, але ім'я вводити тільки англійською, наприклад: \n/hero axe, \n /hero 1, \nабо список усіх героїв: /heroes");
                    break;
                case "🧑‍💻 Гравці":
                    await _botClient.SendMessage(message.Chat.Id, "Введіть ID гравця /player <id> або використайте команду /players для списку.");
                    break;
                case "🏆 Команди":
                    await _botClient.SendMessage(message.Chat.Id, "Введіть ID команди або її назву, наприклад: \n/team Thunder Predator, \n /team 9080405, \nабо список усіх команд: /teams\"");
                    break;
                case "📝 Останній патч":
                    await HandleLatestPatch(message.Chat.Id);
                    break;
                case "🔔 Підписки":
                    await SubscribeMenu(message.Chat.Id);
                    break;
                case "➕ Додати підписку":
                    await _botClient.SendMessage(message.Chat.Id, "Щоб підписатися на оновлення, використайте команду /subscribe <id героя> <id команди>. \nЯкщо хочете підписатись на оновлення нового патчу, використайте /subscribe <id героя> <id команди> <true>");
                    break;
                case "🎬 GIF":
                    await _botClient.SendMessage(message.Chat.Id, "Введіть /gif <тег>, щоб знайти гіфку за темою, але обов'язково пишіть англійською мовою, \nнаприклад: /gif dota2");
                    break;
                case "🎲 Випадковий Герой":
                    await HandleRandomHero(message.Chat.Id);
                    break;
                case "⚔️ Останні Матчі":
                    await HandleRecentMatches(message.Chat.Id);
                    break;
                case "🏆 Топ-10 гравців":
                    await HandleTopPlayers(message.Chat.Id);
                    break;
                case "📋 Показати підписку":
                    await HandleShowSubscribe(message.Chat.Id);
                    break;
                case "🗑️ Видалити останню підписку":
                    await HandleDeleteLastSubscribe(message.Chat.Id);
                    break;
                case "⭐ Улюблені герої":
                    await HandleFavouriteHeroes(message.Chat.Id);
                    break;
                case "⭐ Улюблені команди":
                    await HandleFavouriteTeams(message.Chat.Id);
                    break;
                default:
                    await HandleCustomCommands(message);
                    break;
            }
        }

        private async Task HandleCustomCommands(Message message)
        {
            if (message.Text!.StartsWith("/gif"))
            {
                await HandleGif(message);
                return;
            }
            if (message.Text.StartsWith("/subscribe"))
            {
                await HandleCreateSubscribe(message);
                return;
            }
            if (message.Text.StartsWith("/team"))
            {
                await HandleTeam(message);
                return;
            }
            if (message.Text.StartsWith("/hero"))
            {
                await HandleHero(message);
                return;
            }
            if (message.Text.StartsWith("/player"))
            {
                await HandlePlayer(message);
                return;
            }
        }

        private async Task MainMenu(long chatId)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "🦸 Герої", "🧑‍💻 Гравці" },
                new KeyboardButton[] { "🏆 Команди", "⚔️ Останні Матчі" },
                new KeyboardButton[] { "📝 Останній патч", "🔔 Підписки" },
                new KeyboardButton[] { "🎬 GIF", "🎲 Випадковий Герой" },
                new KeyboardButton[]  { "🏆 Топ-10 гравців" },
                new KeyboardButton[] { "⭐ Улюблені герої", "⭐ Улюблені команди" }
            })
            {
                ResizeKeyboard = true
            };
            await _botClient.SendMessage(chatId, "Оберіть поле", replyMarkup: replyKeyboard);
        }

        private async Task SubscribeMenu(long chatId)
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
            await _botClient.SendMessage(chatId, "Оберіть поле", replyMarkup: replyKeyboard);
        }

        private async Task HandleLatestPatch(long chatId)
        {
            var response1 = await _httpClient.GetAsync("api/Patch/GetLatestPatch");
            var response2 = await _httpClient.GetAsync("api/Patch/GetLatestNotes");
            if (!response1.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(chatId, $"Помилка API: {(int)response1.StatusCode}");
                return;
            }
            var patch = await response1.Content.ReadFromJsonAsync<PatchList>();
            var patchNotes = await response2.Content.ReadFromJsonAsync<PatchNotes>();
            if (patch == null)
            {
                await _botClient.SendMessage(chatId, "Немає даних про останній патч.");
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
            string text = sb.ToString();

            await SendLongMessage(chatId, text);
        }

        private async Task HandleRandomHero(long chatId)
        {
            var response = await _httpClient.GetAsync("api/Hero/GetRandomHero");
            var randomHero = await response.Content.ReadFromJsonAsync<Hero>();
            if (randomHero == null)
            {
                var randomHeroAgain = await response.Content.ReadFromJsonAsync<Hero>();
                await _botClient.SendMessage(chatId, $"Дані для {randomHeroAgain.LocalizedName}\n— Його атрибут: {randomHeroAgain.PrimaryAttr}\n— Його ID: {randomHeroAgain.Id}");
                return;
            }
            await _botClient.SendMessage(chatId, $"Дані для {randomHero.LocalizedName}\n— Його атрибут: {randomHero.PrimaryAttr}\n— Його ID: {randomHero.Id}");
        }

        private async Task HandleRecentMatches(long chatId)
        {
            var response = await _httpClient.GetAsync("api/Match/GetRecentMatches");
            if (!response.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(chatId, $"Помилка API: {(int)response.StatusCode}");
                return;
            }
            var matches = await response.Content.ReadFromJsonAsync<List<ProMatches>>();
            if (matches == null || matches.Count == 0)
            {
                await _botClient.SendMessage(chatId, "Немає даних про матчі.");
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
                sb.AppendLine($"Тривалість: {TimeSpan.FromSeconds(match.Duration):hh\\:mm\\:ss}");
            }
            await _botClient.SendMessage(chatId, sb.ToString());
        }

        private async Task HandleTopPlayers(long chatId)
        {
            var response = await _httpClient.GetAsync("api/Player/GetLeaderBoardDatabase");
            if (!response.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(chatId, "Немає даних у топі гравців");
                return;
            }
            var leaderboard = await response.Content.ReadFromJsonAsync<List<PlayerRankCache>>();
            if (leaderboard == null || leaderboard.Count == 0)
            {
                await _botClient.SendMessage(chatId, "Немає даних у топі гравців");
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine("Топ 10 гравців:");
            foreach (var player in leaderboard)
            {
                sb.AppendLine(new string('-', 20));
                sb.AppendLine($"\nНік: {player.PersonaName}");
                sb.AppendLine($"Аккаунт айді: {player.AccountId}");
                sb.AppendLine($"Ранг у світовій таблиці: {player.Rank}");
            }
            await _botClient.SendMessage(chatId, sb.ToString());
        }


        private async Task HandleGif(Message message)
        {
            var parts = message.Text.Split(' ', 2);
            if (parts.Length < 2)
            {
                await _botClient.SendMessage(message.Chat.Id, "Використайте: /gif <тег>");
                return;
            }
            var gifName = parts[1].Trim();
            var response = await _httpClient.GetAsync($"api/Gif/GetByTag?tag={gifName}");
            if (!response.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Ви ввели тег не англійською мовою, спробуйте ще раз");
                return;
            }
            var gif = await response.Content.ReadFromJsonAsync<Giphy.RandomGiphy>();

            if (string.IsNullOrEmpty(gif?.Data?.Images?.FixedHeight?.Url))
            {
                await _botClient.SendMessage(message.Chat.Id, "Гіфка за цим тегом не знайдена.");
                return;
            }
            await _botClient.SendAnimation(message.Chat.Id, gif.Data.Images.FixedHeight.Url, caption: $"Гіфка за тегом: {gifName}");
        }

        private async Task HandleCreateSubscribe(Message message)
        {
            var parts = message.Text.Split(' ', 4);
            if (parts.Length < 3)
            {
                await _botClient.SendMessage(message.Chat.Id, "Використайте: /subscribe <id_героя> <id_команди>");
                return;
            }
            if (!int.TryParse(parts[1], out int heroId) || !int.TryParse(parts[2], out int teamId))
            {
                await _botClient.SendMessage(message.Chat.Id, "ID героя та команди мають бути числами.");
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
            var response = await _httpClient.PostAsJsonAsync("api/Subscribe/Subscribe", subscribe);
            if (response.IsSuccessStatusCode && !patch)
                await _botClient.SendMessage(message.Chat.Id, $"Ви підписалися на героя з ID: {heroId} та команду з ID: {teamId}");
            else if (response.IsSuccessStatusCode && patch)
                await _botClient.SendMessage(message.Chat.Id, $"Ви підписалися на героя з ID: {heroId} та команду з ID: {teamId}, а також на оновлення патчу");
            else
                await _botClient.SendMessage(message.Chat.Id, "Помилка при підписці.");
        }

        private async Task HandleShowSubscribe(long chatId)
        {
            var response = await _httpClient.GetAsync($"api/Subscribe/GetById?id={chatId}");
            if (!response.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(chatId, "Помилка при отриманні підписок.");
                return;
            }
            var subs = await response.Content.ReadFromJsonAsync<List<Subscribe>>();
            if (subs == null)
            {
                await _botClient.SendMessage(chatId, "У вас немає підписок.");
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine("Ваши подписки:");
            foreach (var sub in subs)
            {
                sb.AppendLine($"Герой ID: {sub.FavouriteHeroId}, Команда ID: {sub.FavouriteTeamId}");
            }
            var isSubscribedForPatch = subs.Any(x => x.IsSubscribeForPatch);
            await _botClient.SendMessage(chatId, sb.ToString() + $"Чи підписані на патч: {isSubscribedForPatch}");
        }

        private async Task HandleDeleteLastSubscribe(long chatId)
        {
            var response = await _httpClient.DeleteAsync($"api/Subscribe/DeleteSubscribe?id={chatId}");
            if (response.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(chatId, "Ваша остання підписка видалена.");
            }
            else
            {
                await _botClient.SendMessage(chatId, "Помилка при видаленні підписки або у вас немає підписок.");
            }
        }
        private async Task HandleFavouriteHeroes(long chatId)
        {
            var response = await _httpClient.GetAsync($"api/Subscribe/GetById?id={chatId}");
            if (!response.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(chatId, "Не вдалося отримати улюблених героїв.");
                return;
            }
            var subs = await response.Content.ReadFromJsonAsync<List<Subscribe>>();
            if (subs == null || subs.Count == 0)
            {
                await _botClient.SendMessage(chatId, "У вас немає улюблених героїв.");
                return;
            }
            var heroIds = subs
                .Where(s => s.FavouriteHeroId != 0)
                .GroupBy(s => s.FavouriteHeroId)
                .Select(g => g.Key)
                .ToList();

            if (heroIds.Count == 0)
            {
                await _botClient.SendMessage(chatId, "У вас немає улюблених героїв.");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Ваші улюблені герої:");
            foreach (var heroId in heroIds)
            {
                var heroResponse = await _httpClient.GetAsync($"api/Hero/GetHeroById?id={heroId}");
                if (heroResponse.IsSuccessStatusCode)
                {
                    var hero = await heroResponse.Content.ReadFromJsonAsync<Hero>();
                    if (hero != null) 
                    {
                        sb.AppendLine($"- Герой: {hero.LocalizedName}");
                        sb.AppendLine($"- ID: { hero.Id}");
                        sb.AppendLine($"Атрибут: " + GetAttributeDisplayName(hero.PrimaryAttr));
                        sb.AppendLine();
                    }
                }else
                    await _botClient.SendMessage(chatId, "Команда з таким айді немає або ви вказали неправильний айді при підписці.");
            }
            await _botClient.SendMessage(chatId, sb.ToString());
        }
        private async Task HandleFavouriteTeams(long chatId)
        {
            var response = await _httpClient.GetAsync($"api/Subscribe/GetById?id={chatId}");
            if (!response.IsSuccessStatusCode)
            {                await _botClient.SendMessage(chatId, "Не вдалося отримати улюблені команди.");
                return;
            }
            var subs = await response.Content.ReadFromJsonAsync<List<Subscribe>>();
            if (subs == null || subs.Count == 0)
            {
                await _botClient.SendMessage(chatId, "У вас немає улюблених команд.");
                return;
            }
            var teamIds = subs
                .Where(s => s.FavouriteTeamId != 0)
                .GroupBy(s => s.FavouriteTeamId)
                .Select(g => g.Key)
                .ToList();

            if (teamIds.Count == 0)
            {
                await _botClient.SendMessage(chatId, "У вас немає улюблених команд.");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Ваші улюблені команди:");
            foreach (var teamId in teamIds)
            {
                var teamResponse = await _httpClient.GetAsync($"api/Team/GetTeamById?id={teamId}");
                if (teamResponse.IsSuccessStatusCode)
                {
                        var team = await teamResponse.Content.ReadFromJsonAsync<Team>();

                        if (team != null)
                        {
                            sb.AppendLine(team.Name);
                            sb.AppendLine($"- Тег: {team.Tag}");
                            sb.AppendLine($"- ID: {team.TeamId}");
                            sb.AppendLine();
                        }
                }else
                    await _botClient.SendMessage(chatId, "Одна з команд що ви вказалі при підписці немає або ви вказали неправильний айді.");
            }
            await _botClient.SendMessage(chatId, sb.ToString());
        }

        private async Task HandleTeam(Message message)
        {
            if (message.Text == "/teams")
            {
                var response = await _httpClient.GetAsync("api/Team/GetAllTeams");
                if (!response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                var teams = await response.Content.ReadFromJsonAsync<List<Team>>();
                if (teams == null || teams.Count == 0)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Немає даних про команди.");
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
                await SendLongMessage(message.Chat.Id, sb.ToString());
                return;
            }
            var parts = message.Text.Split(' ', 2);
            if (parts.Length < 2)
            {
                await _botClient.SendMessage(message.Chat.Id, "Використайте: /team <name> або /team <id>");
                return;
            }
            var teamInput = parts[1].Trim();
            if (int.TryParse(teamInput, out int teamId))
            {
                var sb = new StringBuilder();
                var response = await _httpClient.GetAsync($"api/Team/GetTeamById?id={teamId}");
                if (!response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                var team = await response.Content.ReadFromJsonAsync<Team>();

                sb.AppendLine("Інформація про команду:");
                sb.AppendLine($"Назва: {team.Name}");
                sb.AppendLine($"ID: {team.TeamId}");
                sb.AppendLine($"Тег: {team.Tag}");
                sb.AppendLine($"Рейтинг: {team.Rating}");
                sb.AppendLine($"Перемоги: {team.Wins}");
                sb.AppendLine($"Поразки: {team.Losses}");
                await _botClient.SendMessage(message.Chat.Id, sb.ToString());
            }
            else
            {
                var response = await _httpClient.GetAsync($"api/Team/GetTeamByName?name={teamInput}");
                if (!response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                if ((int)response.StatusCode == 204)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Команди з такою назвою не має, спробуйте ще раз.");
                    return;
                }
                var team = await response.Content.ReadFromJsonAsync<Team>();
                if (team == null)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Команду з такою назвою не знайдено, спробуйте ще раз.");
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
                await _botClient.SendMessage(message.Chat.Id, sb.ToString());
            }
        }

        private async Task HandleHero(Message message)
        {

            if (message.Text == "/heroes")
            {
                var response = await _httpClient.GetAsync("api/Hero/GetAllHeroes");
                if (!response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                var heroes = await response.Content.ReadFromJsonAsync<List<Hero>>();
                if (heroes == null || heroes.Count == 0)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Немає даних про героїв.");
                    return;
                }
                var sb = new StringBuilder();
                sb.AppendLine("Список героїв:");
                int i = 1;
                foreach (var hero in heroes)
                {
                    sb.AppendLine(new string('-', 20));
                    sb.AppendLine($"{i++}.");
                    sb.AppendLine($"Назва: {hero.LocalizedName}");
                    sb.AppendLine($"ID: {hero.Id}");
                    sb.AppendLine($"Атрибут: " + GetAttributeDisplayName(hero.PrimaryAttr));
                    sb.AppendLine($"Тип атаки: {hero.AttackType}");
                }
                await SendLongMessage(message.Chat.Id, sb.ToString());
                return;
            }
            var parts = message.Text.Split(' ', 2);
            if (parts.Length < 2)
            {
                await _botClient.SendMessage(message.Chat.Id, "Використайте: /hero <name> або /hero <id>");
                return;
            }
            var heroInput = parts[1].Trim();
            if (int.TryParse(heroInput, out int heroId))
            {
                var sb = new StringBuilder();
                var response = await _httpClient.GetAsync($"api/Hero/GetHeroById?id={heroId}");
                if (!response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                if ((int)response.StatusCode == 204)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Героя з таким ID не знайдено, спробуйте ще раз");
                    return;
                }
                var findHero = await response.Content.ReadFromJsonAsync<Hero>();

                sb.AppendLine($"- Дані для {findHero.LocalizedName}");
                sb.AppendLine($"— Його атрибут: " + GetAttributeDisplayName(findHero.PrimaryAttr));
                sb.AppendLine($"- Тип атаки: {findHero.AttackType}");
                await _botClient.SendMessage(message.Chat.Id, sb.ToString());
            }
            else
            {
                var sb = new StringBuilder();
                var response = await _httpClient.GetAsync($"api/Hero/GetHeroByName?name={heroInput.ToLower()}");
                if (!response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                if ((int)response.StatusCode == 204)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Героя з таким ім'ям не знайдено або ви написали не англійською, спробуйте ще раз");
                    return;
                }
                var findHero = await response.Content.ReadFromJsonAsync<Hero>();
                
                sb.AppendLine($"- Дані для {findHero.LocalizedName}");
                sb.AppendLine($"— Його атрибут: " + GetAttributeDisplayName(findHero.PrimaryAttr));
                sb.AppendLine($"— Його ID: {findHero.Id}");
                sb.AppendLine($"- Тип атаки: {findHero.AttackType}");
                await _botClient.SendMessage(message.Chat.Id, sb.ToString());
            }
        }

        private async Task HandlePlayer(Message message)
        {
            if (message.Text == "/players")
            {
                var response = await _httpClient.GetAsync("api/Player/GetProPlayers");
                if (!response.IsSuccessStatusCode)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)response.StatusCode}");
                    return;
                }
                var players = await response.Content.ReadFromJsonAsync<List<ProPlayer>>();
                if (players == null || players.Count == 0)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Немає даних про гравців.");
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
                await SendLongMessage(message.Chat.Id, sb.ToString());
                return;
            }
            var parts = message.Text.Split(' ', 2);
            if (parts.Length < 2)
            {
                await _botClient.SendMessage(message.Chat.Id, "Використайте: /player <id>");
                return;
            }
            var playerId = parts[1];
            var responsePlayer = await _httpClient.GetAsync($"api/Player/GetPlayerById?id={playerId}");
            if (!responsePlayer.IsSuccessStatusCode)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка API: {(int)responsePlayer.StatusCode}");
                return;
            }
            var playersData = await responsePlayer.Content.ReadFromJsonAsync<Player>();
            if (playersData == null)
            {
                await _botClient.SendMessage(message.Chat.Id, "Немає даних про гравця.");
                return;
            }
            var sbPlayer = new StringBuilder();
            sbPlayer.AppendLine($"Інформація про гравця:");
            sbPlayer.AppendLine($"Нік: {playersData.Profile.Name}");
            sbPlayer.AppendLine($"ID: {playersData.Profile.AccountId}");
            sbPlayer.AppendLine($"Ранг у світі: {playersData.LeaderboardRank}");
            sbPlayer.AppendLine($"Країна: {playersData.Profile.LocCountryCode}");
            sbPlayer.AppendLine($"Останній вхід: {playersData.Profile.LastLogin}");

            await _botClient.SendPhoto(message.Chat.Id, playersData.Profile.Avatar!, caption: sbPlayer.ToString());
        }

        private async Task SendLongMessage(long chatId, string text)
        {
            if (text.Length <= TelegramMessageLimit)
            {
                await _botClient.SendMessage(chatId, text);
            }
            else
            {
                for (int i = 0; i < text.Length; i += TelegramMessageLimit)
                {
                    var part = text.Substring(i, Math.Min(TelegramMessageLimit, text.Length - i));
                    await _botClient.SendMessage(chatId, part);
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
                    var response = await _httpClient.GetAsync("api/Patch/GetLatestPatch");
                    if (response.IsSuccessStatusCode)
                    {
                        var patch = await response.Content.ReadFromJsonAsync<LastPatch>();
                        if (patch != null && patch.LatestPatch != _lastPatchVersion)
                        {
                            _lastPatchVersion = patch.LatestPatch;

                            var subsResponse = await _httpClient.GetAsync("api/Subscribe/GetAll");
                            var noteResponse = await _httpClient.GetAsync("api/Patch/GetLatestNotes");
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
                                    if (!subscribedForPatch.Contains(chatId) && group.Any(sub => sub.IsSubscribeForPatch == true))
                                    {
                                        await _botClient.SendMessage(chatId, $"Вийшов новий патч: {patch.LatestPatch}!");
                                        subscribedForPatch.Add(chatId);
                                    }
                                    foreach (var sub in group)
                                    {
                                        var heroSection = heroes.FirstOrDefault(x => x.HeroId == sub.FavouriteHeroId);
                                        if (heroSection != null)
                                        {
                                            var result = FormatHeroSection(heroSection);
                                            await SendLongMessage(chatId, result);
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
                sb.AppendLine("Помітки героя:");
                foreach (var note in heroSection.HeroNotes)
                    sb.AppendLine($"- {note.Note}");
            }

            if (heroSection.TalentNotes != null && heroSection.TalentNotes.Count > 0)
            {
                sb.AppendLine("Таланти:");
                foreach (var note in heroSection.TalentNotes)
                    sb.AppendLine($"- {note.Note}");
            }

            if (heroSection.Abilities != null && heroSection.Abilities.Count > 0)
            {
                sb.AppendLine("Зміна здібностей:");
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
                    sb.AppendLine($"Вроджена здатність: {sub.Title}");
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
        private string GetAttributeDisplayName(string? attr)
        {
            return attr switch
            {
                "str" => "Сила",
                "agi" => "Ловкість",
                "int" => "Інтелект",
                _ => attr ?? "Невідомо"
            };
        }
        
    }
}
