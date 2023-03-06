using CSGOChatGPT;
using CSGOChatGPT.CSGO;

Console.WriteLine("CS:GO Chat GPT integration");
Console.WriteLine("By    : porrasm");
Console.WriteLine("GitHub: https://github.com/porrasm/csgo-chat-gpt");
System.Console.WriteLine("\nIf this is your first time starting the program, please type 'config' to configure the bot.\n");
Console.WriteLine("List of commands: start, exit, config, resetconfig");

Config.LoadConfig();

CSGOChatBot bot = null;

while(true) {
    string command = Console.ReadLine();

    if (command == "start") {
        Console.WriteLine("Starting bot...");
        if (bot != null) {
            bot.Dispose();
        }
        bot  = new CSGOChatBot(Config.Instance.PersonalNickname, Config.Instance.TelnetPort, Config.Instance.AIInstruction);
        bot.Start();
        continue;
    }

    if (command == "exit") {
        Console.WriteLine("Exiting...");

        if (bot != null) {
        bot.Dispose();
        }
        break;
    }

    if (command == "config") {
        Console.WriteLine("Opening config...");
        Config.EditConfig();
        break;
    }

    if (command == "resetconfig") {
        Console.WriteLine("Resetting config...");
        Config.ResetConfig();
    }
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
