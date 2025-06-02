using TelegramBot;


var bot = new Dota2TelegramBot();
_ = bot.CheckForNewPatchAsync(); 
await bot.Start();

