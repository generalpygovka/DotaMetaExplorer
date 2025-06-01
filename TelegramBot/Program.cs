using TelegramBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotaMetaExplorer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Додаємо всі потрібні сервіси, наприклад:
        services.AddDbContext<DotaMetaExplorer.Context.ApplicationDBContext>(options =>options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<LeaderboardCacheService>();
        services.AddScoped<Dota2TelegramBot>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var leaderboardService = scope.ServiceProvider.GetRequiredService<LeaderboardCacheService>();
    var cacheLifetime = TimeSpan.FromHours(24); // або інший інтервал

    // Оновлюємо кеш, якщо він застарів
    if (!await leaderboardService.IsCacheActualAsync(cacheLifetime))
    {
        await leaderboardService.UpdateLeaderboardCacheAsync();
    }

    // Запускаємо бота
    var bot = scope.ServiceProvider.GetRequiredService<Dota2TelegramBot>();
    // Запускаємо перевірку патча у фоні
    _ = bot.CheckForNewPatchAsync();
    await bot.Start();
}
