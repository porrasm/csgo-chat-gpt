using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOChatGPT;
public class Config {
    private const string CONFIG_FILE = "./config.json";
    public static Config Instance { get; private set; }


    /// <summary>
    /// Your current Steam nickname. Needed so the AI won't respond to your messages.
    /// </summary>
    public string PersonalNickname = "your_current_nickname";
    
    /// <summary>
    /// Your ChatGPT API key. Get one here: https://platform.openai.com/account/api-keys"
    /// </summary>
    public string APIKey = "your_chatgpt_api_key";

    /// <summary>
    /// The length of the AI responses. 1 token ~ 4 characters.
    /// </summary>
    public int ResponseMaxTokens = 40;

    /// <summary>
    /// Whether the AI should respond to chat messages automatically.
    /// </summary>
    public bool RespondToChatAutomatically = true;

    /// <summary>
    /// Whether the AI should respond to any console message that contains the splitter. Needed for other games than CS:GO or for some community servers. Example: chat message starts with 'ai: ', the bot will respond.
    /// </summary>
    public bool RespondToAnyConsoleMessageWithSplitter = true;

    /// <summary>
    /// The splitter that the AI will use to respond to console messages.
    /// </summary>
    public string ConsoleMessageSplitter = " ai: ";

    /// <summary>
    /// The port you set in the games launch options with the '-netconport' parameter.
    /// </summary>
    public int TelnetPort = 12350;
 
    /// <summary>
    /// How many messages the AI should remember. The more messages, the more accurate the AI will be. But it will also take longer to respond.
    /// </summary>
    public int MessageHistoryLength = 6;

    /// <summary>
    /// The AI instruction. You can experiment with this. Example: 'You are roleplaying as a very angry gamer.' or 'You are a helpful assistant.'
    /// </summary>
    public string AIInstruction = "You are a helpful assistant.";
    

    static Config() {
        Instance = new Config();
    }
    
    public static void LoadConfig() {
        if (File.Exists(CONFIG_FILE)) {
            Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(CONFIG_FILE));
        } else {
            Instance = new Config();
            SaveConfig();
        }
    }

    public static void SaveConfig() {
        File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(Instance, Formatting.Indented));
    }

    public static void ResetConfig() {
        Instance = new Config();
        SaveConfig();
    }

    public static void EditConfig() {
        // Open with default text editor
        Process.Start("notepad.exe", CONFIG_FILE);
    }
}

public class JsonCommentConverter : JsonConverter {
    private readonly string _comment;
    public JsonCommentConverter(string comment) {
        _comment = comment;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        writer.WriteValue(value);
        writer.WriteComment(_comment);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer) {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => false;
}

