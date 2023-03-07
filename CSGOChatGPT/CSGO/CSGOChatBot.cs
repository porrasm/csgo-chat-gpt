using CSGOChatGPT.ChatGPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOChatGPT.CSGO {
    public class CSGOChatBot : IDisposable {
        private string myName;
        public CSGOTelnetClient CS { get; private set; }
        private ChatGPTConversation aiConversation;
        private bool isHandlingMessage = false;
        private bool isPaused = false;
        private const string MESSAGE_PREFIX = "CSGOChatGPT";

        public CSGOChatBot(string myName, int telnetPort, string aiInstruction = null) {
            this.myName = myName;
            this.CS = new CSGOTelnetClient(telnetPort);
            aiConversation = new ChatGPTConversation(Config.Instance.MessageHistoryLength.Value, aiInstruction) { ChatResponseTokenLength = Config.Instance.ResponseMaxTokens.Value };

            DefaultCommands();
        }

        private void DefaultCommands() {
            CS.AdditionalCommands.AddCommand("chatbot_help", "Shows this help message", (param) => {
                PrintToConsole(CS.AdditionalCommands.ToString());
            });
            CS.AdditionalCommands.AddCommand("chatbot", "Sends a chat message to ChatGPT", (param) => {
                if (string.IsNullOrWhiteSpace(param)) {
                    PrintToConsole("Message was empty.");
                    PrintToConsole("Usage: chatbot_chat <message>");
                    return;
                }

                ChatBotTask(param);
            });
            CS.AdditionalCommands.AddCommand("chatbot_private", "Sends a private chat message to ChatGPT and only responds in console.", (param) => {
                if (string.IsNullOrWhiteSpace(param)) {
                    PrintToConsole("Message was empty.");
                    PrintToConsole("Usage: chatbot_chat_private <message>");
                    return;
                }

                ChatBotTask(param, true);
            });
            CS.AdditionalCommands.AddCommand("chatbot_resume", "Resumes the chatbot", (param) => {
                isPaused = false;
                PrintToConsole("Chatbot resumed.");
            });
            CS.AdditionalCommands.AddCommand("chatbot_pause", "Pauses the chatbot", (param) => {
                isPaused = true;
                PrintToConsole("Chatbot paused.");
            });
            CS.AdditionalCommands.AddCommand("chatbot_reset", "Resets the chatbot", (param) => {
                aiConversation.Messages.Clear();
                PrintToConsole("Chatbot history reset.");
            });
            CS.AdditionalCommands.AddCommand("chatbot_history", "Shows the chatbot history", (param) => {
                foreach (var message in aiConversation.Messages) {
                    PrintToConsole(message.ToString());
                }
            });
            CS.AdditionalCommands.AddCommand("chatbot_role", "Sets the chatbot role (AIInstruction). Also clears the history.", (param) => {
                if (string.IsNullOrWhiteSpace(param)) {
                    PrintToConsole("New role was empty.");
                    PrintToConsole("Usage: chatbot_role <role>");
                    return;
                }

                aiConversation.SetInstruction(param);
                PrintToConsole("CSGOChatGPT: Chatbot role set to " + param);
            });
        }

        public void PrintToConsole(string message) {
            CS.EchoLongMessage(message, MESSAGE_PREFIX + ": ");
        }

        public void Start() {
            CS.Connect();
            CS.OnChatMessageReceived += OnChatMessage;
            CS.ResponseReceived += OnReceiveResponse;
        }
        public void Dispose() {
            CS.Dispose();
        }

        private void OnReceiveResponse(string message) {
            if (isPaused || !Config.Instance.RespondToAnyConsoleMessage.Value || message.StartsWith(MESSAGE_PREFIX)) {
                return;
            }

            string messageLower = message.ToLower();
            if (messageLower.Contains(Config.Instance.RespondToAnyConsoleMessageSplitter.Value)) {
                // Find index of OVERRIDE_SPLITTER
                int index = messageLower.IndexOf(Config.Instance.RespondToAnyConsoleMessageSplitter.Value);
                ChatBotTask(message.Substring(index + Config.Instance.RespondToAnyConsoleMessageSplitter.Value.Length));
            }
        }

        private void OnChatMessage(string name, string message) {
            if (isPaused 
            || !Config.Instance.RespondToChatAutomatically.Value 
            || name.StartsWith(MESSAGE_PREFIX) 
            || message.StartsWith(MESSAGE_PREFIX)) {
                return;
            }

            bool forceOverride = false;
            if (message.ToLower().StartsWith("ai: ")) {
                forceOverride = true;
                message = message.Substring(4);
            }

            if (!forceOverride && name == myName) {
                return;
            }

            ChatBotTask(message);
        }

        public async Task ChatBotTask(string message, bool onlyConsoleResponse = false) {
            if (isHandlingMessage) {
                return;
            }

            isHandlingMessage = true;

            try {
                var response = await aiConversation.GenerateResponse(message);

                if (response == null) {
                    Console.WriteLine("Chat GPT request failed");
                    return;
                }

                Console.WriteLine("Received AI response: " + response.content);

                if (!onlyConsoleResponse && Config.Instance.RespondUsingGameChat.Value) {
                    await CS.SendChat(response.content);
                } 
                
                PrintToConsole("ChatGPT response:\n---------------------------------------------------------\n" + response.content + "\n---------------------------------------------------------");
            } catch (Exception e) {
                Console.WriteLine("Error in ChatBotTask: " + e.Message);
            } finally {
                isHandlingMessage = false;
            }
        }
    }
}
