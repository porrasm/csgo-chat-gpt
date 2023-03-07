using CSGOChatGPT;
using CSGOChatGPT.CSGO;

Console.WriteLine("CS:GO Chat GPT integration");
Console.WriteLine("By    : porrasm");
Console.WriteLine("GitHub: https://github.com/porrasm/csgo-chat-gpt");
Console.WriteLine("\nIf this is your first time starting the program, please type 'config' to configure the bot.\n");

Config.LoadConfig();

CSGOChatBot bot = null;

CommandSet commandSet = new CommandSet();
commandSet.AddCommand("help", "Shows this help message", (string parameter) => {
    Console.WriteLine(commandSet.ToString());
});
commandSet.AddCommand("start", "Starts the bot", (string parameter) => {
    Console.WriteLine("Starting bot...");
    if (bot != null) {
        bot.Dispose();
    }
    bot  = new CSGOChatBot(Config.Instance.PersonalNickname.Value, Config.Instance.TelnetPort.Value, Config.Instance.AIInstruction.Value);
    bot.Start();
});
commandSet.AddCommand("exit", "Exits the program", (string parameter) => {
    Console.WriteLine("Exiting...");
    if (bot != null) {
        bot.Dispose();
    }
    Environment.Exit(0);
});
commandSet.AddCommand("config", "Opens the config file", (string parameter) => {
    Console.WriteLine("Opening config...");
    Config.EditConfig();
});
commandSet.AddCommand("resetconfig", "Resets the config file", (string parameter) => {
    Console.WriteLine("Resetting config...");
    Config.ResetConfig();
});
commandSet.AddCommand("chat", "Send a message to ChatGPT", (string parameter) => {
    if (bot == null) {
        Console.WriteLine("Bot is not running");
        return;
    }
    bot.ChatBotTask(parameter);
});

Console.WriteLine("\n" + commandSet);
Console.WriteLine("\n\nAdditional commands are available through the game console. Type 'echo chatbot_help' in the game console.\n\n");

if (Config.Instance.ConnectAtStartup.Value) {
    commandSet.ExecuteCommand("start");
}

while(true) {
    string? command = Console.ReadLine();
    if (command == null) {
        continue;
    }
    if (!commandSet.ExecuteCommand(command)) {
        Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
    }    
}
