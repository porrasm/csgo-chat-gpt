using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSGOChatGPT;
public class Config {
    private const string CONFIG_FILE = "./config.cfg";
    public static Config Instance { get; private set; }


    public ConfigValue<bool> ConnectAtStartup { get; set; } = new ConfigValue<bool>(false, "Whether the bot should connect to the game at startup.");
    public ConfigValue<string> PersonalNickname { get; set; } = new ConfigValue<string>("your_current_nickname", "Your current Steam nickname. Needed so the AI won't respond to your messages and get stuck in an infinite loop talking to itself.");
    public ConfigValue<string> APIKey { get; set; } = new ConfigValue<string>("your_chatgpt_api_key", "Your ChatGPT API key. Get one here: https://platform.openai.com/account/api-keys");
    public ConfigValue<int> TelnetPort { get; set; } = new ConfigValue<int>(12350, "The port you set in the games launch options with the '-netconport' parameter.");
    public ConfigValue<int> GameChatMessageMaxLength { get; set; } = new ConfigValue<int>(100, "The maximum length of a chat message in the game. If a message is longer than this, the messages will be split into multiple chunks. Your username length will affect this.");
    public ConfigValue<bool> RespondToChatAutomatically { get; set; } = new ConfigValue<bool>(true, "Whether the AI should respond to chat messages automatically.");
    public ConfigValue<string> RespondToChatSplitter { get; set; } = new ConfigValue<string>("‎ : ", "The string that separates the nickname from the message. Notice the hidden character in the default value: '[U+200E] : '.");
    public ConfigValue<string> RespondToChatDeadSplitter { get; set; } = new ConfigValue<string>("*DEAD*", "The string that appears in fron of your name when you're dead.");
    public ConfigValue<bool> RespondToAnyConsoleMessage { get; set; } = new ConfigValue<bool>(true, "Whether the AI should respond to any console message that contains the splitter. Needed for other games than CS:GO or for some community servers. Example: chat message starts with 'ai: ', the bot will respond.");
    public ConfigValue<string> RespondToAnyConsoleMessageSplitter { get; set; } = new ConfigValue<string>(" ai: ", "The splitter that the AI will use to respond to console messages.");
    public ConfigValue<bool> RespondUsingGameChat { get; set; } = new ConfigValue<bool>(true, "Whether the AI should respond to using the game chat. Set to false to receive AI responses only in the game console. Additionally set the automatic responses from players to false and you can use the console as a personal ChatGPT interface without interference from other players.");
    public ConfigValue<string> AIInstruction { get; set; } = new ConfigValue<string>("You are a helpful assistant.", "The AI instruction. You can experiment with this. Example: 'You are roleplaying as a very angry gamer.' or 'You are a helpful assistant.'");
    public ConfigValue<int> MessageHistoryLength { get; set; } = new ConfigValue<int>(6, "How many messages the AI should remember. The more messages, the more accurate the AI will be. But it will also take longer to respond.");
    public ConfigValue<bool> IgnoreAIMessagesInHistory { get; set; } = new ConfigValue<bool>(false, "Whether the AI should ignore messages that it sent itself. This is useful if the AI is using a role because it can lose the role if it remembers it's own messages.");
    public ConfigValue<int> ResponseMaxTokens { get; set; } = new ConfigValue<int>(0, "The length of the AI responses. 1 token ~ 4 characters.");

    static Config() {
        Instance = new Config();
    }

    private static object ParseConfigValue(PropertyInfo property, string propertyValue) {
        var valueType = property.PropertyType.GenericTypeArguments[0];
        var trimmedValue = propertyValue.Trim().Replace("\r", "").Replace("\n", ""); // remove any newline characters
        var value = Convert.ChangeType(trimmedValue, valueType);
        return value;
    }

    public static void LoadConfig() {
        Console.WriteLine("Loading config...");
        try {
            if (File.Exists(CONFIG_FILE)) {
                Instance = FromString(File.ReadAllText(CONFIG_FILE));
                // Needed so that new values are added to the config file
                SaveConfig();
            } else {
                Instance = new Config();
                SaveConfig();
            }

            Console.WriteLine("Config loaded.");
        } catch (Exception e) {
            Console.WriteLine("Error loading config: " + e.Message);
            Console.WriteLine("The config may be invalid, try resetting it with the 'reset' command.");
        }
    }

    public static void SaveConfig() {
        string configString = Instance.ToString();
        // normalize line endings
        configString = configString.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        File.WriteAllText(CONFIG_FILE, configString);
    }


    public static void ResetConfig() {
        Instance = new Config();
        SaveConfig();
    }

    public static void EditConfig() {
        // Open with default text editor
        Process.Start("notepad.exe", CONFIG_FILE);
    }

    public PropertyInfo[] GetProperties() => GetType().GetProperties(
                          BindingFlags.Public |
                          BindingFlags.NonPublic |
                          BindingFlags.Instance |
                          BindingFlags.DeclaredOnly);

    override public string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine("# CSGOChatGPT config file");
        foreach (var prop in GetProperties()) {
            if (true || prop.PropertyType == typeof(ConfigValue<>)) {
                var configValue = prop.GetValue(this);
                sb.AppendLine($"\n# {((dynamic)configValue).Description}");
                sb.AppendLine($"{prop.Name}={((dynamic)configValue).Value}");
            }
        }
        return sb.ToString();
    }

    public static Config FromString(string configString) {
        var config = new Config();
        var lines = configString.Split('\n');
        foreach (var line in lines) {
            try {


                if (line.StartsWith("#")) continue;
                if (string.IsNullOrWhiteSpace(line)) continue;
                var split = line.Split('=');
                var propertyName = split[0];
                var propertyValue = split[1];
                var property = config.GetType().GetProperty(propertyName);
                if (property == null) {
                    Console.WriteLine($"Unknown property: {propertyName}");
                    continue;
                }
                // var configValueType = property.PropertyType.GenericTypeArguments[0];
                // var value = Convert.ChangeType(propertyValue.Trim(), configValueType);
                var value = ParseConfigValue(property, propertyValue);
                var description = ((dynamic)property.GetValue(config)).Description;
                var configValue = Activator.CreateInstance(property.PropertyType, value, description);
                property.SetValue(config, configValue);
            } catch (Exception e) {
                Console.WriteLine("err: " + e);
            }
        }
        return config;
    }

}

public class ConfigValue<T> {
    public string Description;
    public T Value;

    public ConfigValue(T value, string description) {
        Value = value;
        Description = description;
    }
}