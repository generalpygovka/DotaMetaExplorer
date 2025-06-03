using TelegramBot;

Dota2TelegramBot dota2TelegramBot = new Dota2TelegramBot();
dota2TelegramBot.Start();
await dota2TelegramBot.CheckForNewPatchAsync();
Console.ReadKey();