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

        public CSGOChatBot(string myName, int telnetPort, string aiInstruction = null) {
            this.myName = myName;
            this.CS = new CSGOTelnetClient(telnetPort);
            aiConversation = new ChatGPTConversation(Config.Instance.MessageHistoryLength, aiInstruction) { ChatResponseTokenLength = Config.Instance.ResponseMaxTokens };
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
            if (!Config.Instance.RespondToAnyConsoleMessageWithSplitter) {
                return;
            }

            string messageLower = message.ToLower();
            if (messageLower.Contains(Config.Instance.ConsoleMessageSplitter)) {
                // Find index of OVERRIDE_SPLITTER
                int index = messageLower.IndexOf(Config.Instance.ConsoleMessageSplitter);
                ChatBotTask(message.Substring(index + Config.Instance.ConsoleMessageSplitter.Length));
            }
        }

        private void OnChatMessage(string name, string message) {
            if (!Config.Instance.RespondToChatAutomatically) {
                return;
            }

            bool forceOverride = false;
            if (message.StartsWith("AI: ")) {
                forceOverride = true;
                message = message.Substring(4);
            }

            if (!forceOverride && name == myName) {
                return;
            }

            ChatBotTask(message);
        }

        public async Task ChatBotTask(string message) {
            if (aiConversation.IsGenerating) {
                return;
            }

            try {

                var response = await aiConversation.GenerateResponse(message);

                if (response == null) {
                    Console.WriteLine("Chat GPT request failed");
                    return;
                }

                await CS.SendChat(response.content);

            } catch (Exception e) {
                await CS.SendChat("Error in chat bot :(");
            }
        }
    }
}
