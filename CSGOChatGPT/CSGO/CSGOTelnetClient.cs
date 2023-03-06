using CSGOChatGPT.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOChatGPT.CSGO {
    public class CSGOTelnetClient : IDisposable {
        private const string CHAT_SPLITTER = "‎ : ";
        private const string DEAD_SPLITTER = "*DEAD*";
        private const int CHAT_CHARACTER_LIMIT = 100;

        private TelnetClient client;

        public Action<string> ResponseReceived { get; set; }
        public Action<string, string> OnChatMessageReceived { get; set; }

        public CSGOTelnetClient(int port) {
            client = new TelnetClient("127.0.0.1", port);
        }

        public void Connect() {
            client.Connect();
            client.OnResponseReceived = OnReceive;
        }

        public void Dispose() {
            client.Dispose();
        }

        public void SendCommand(string command) {
            client.SendCommand(command);
        }
        public async Task SendChat(string message) {
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            // Replace all line breaks with spaces
            message = message.Replace("\n", " ");

            // Split message into chunks with space or line break
            string[] words = message.Split(' ');

            StringBuilder currentMessage = new StringBuilder();

            List<string> messageChunks = new List<string>();

            foreach (string word in words) {
                if (currentMessage.Length + word.Length + 1 > CHAT_CHARACTER_LIMIT) {
                    messageChunks.Add(currentMessage.ToString());
                    currentMessage.Clear();
                }

                currentMessage.Append(word + " ");
            }

            messageChunks.Add(currentMessage.ToString());

            foreach (string messageChunk in messageChunks) {
                SendCommand("say " + messageChunk);
                await Task.Delay(1000);
            }
        }

        private void OnReceive(string message) {
            Console.WriteLine("    Game console: " + message);
            ResponseReceived?.Invoke(message);

            if (message.Contains(CHAT_SPLITTER)) {
                string[] split = message.Split(new string[] { CHAT_SPLITTER }, StringSplitOptions.None);
                string name = split[0];

                if (name.Contains(DEAD_SPLITTER)) {
                    name = name.Replace(DEAD_SPLITTER, "");
                }

                name = name.Trim();

                OnChatMessageReceived?.Invoke(name, split[1]);
            }
        }
    }
}
