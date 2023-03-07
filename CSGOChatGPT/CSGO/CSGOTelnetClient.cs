using CSGOChatGPT.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSGOChatGPT.CSGO {
    public class CSGOTelnetClient : IDisposable {
        private TelnetClient client;

        public Action<string>? ResponseReceived { get; set; }
        public Action<string, string>? OnChatMessageReceived { get; set; }

        public CommandSet AdditionalCommands { get; set; } = new CommandSet("Commands (use format 'echo command parameter'):");

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
        public void Echo(string message) {
            client.SendCommand("echo " + message);
        }

        public async Task EchoLongMessage(string message, string prefix = "") {
            var chunks = SplitIntoChunks(message, 100, true);
            foreach (var chunk in chunks) {
                Echo(prefix + chunk);
                await Task.Delay(100);
            }
        }

        public async Task SendChat(string message) {
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            var messageChunks = SplitIntoChunks(message, Config.Instance.GameChatMessageMaxLength.Value, false);

            foreach (string messageChunk in messageChunks) {
                SendCommand("say " + messageChunk);
                await Task.Delay(1000);
            }
        }

        public static List<string> SplitIntoChunks(string message, int chunkSize, bool splitOnLineBreaks) {
            List<string> messageChunks = new List<string>();

            string[] lines = message.Split('\n');

            foreach (string line in lines) {
                if (line.Length > chunkSize) {
                    string[] words = line.Split(' ');

                    StringBuilder currentMessage = new StringBuilder();

                    foreach (string word in words) {
                        if (currentMessage.Length + word.Length + 1 > chunkSize) {
                            messageChunks.Add(currentMessage.ToString());
                            currentMessage.Clear();
                        }

                        currentMessage.Append(word + " ");
                    }

                    messageChunks.Add(currentMessage.ToString());
                } else {
                    messageChunks.Add(line);
                }
            }

            return messageChunks;
        }


        private void OnReceive(string message) {
            Console.WriteLine("    Game console: " + message);
            if (AdditionalCommands.ExecuteCommand(message)) {
                return;
            }

            ResponseReceived?.Invoke(message);

            if (message.Contains(Config.Instance.RespondToChatSplitter.Value)) {
                string[] split = message.Split(new string[] { Config.Instance.RespondToChatSplitter.Value }, StringSplitOptions.None);
                string name = split[0];

                if (!string.IsNullOrEmpty(Config.Instance.RespondToChatDeadSplitter.Value) && name.Contains(Config.Instance.RespondToChatDeadSplitter.Value)) {
                    name = name.Replace(Config.Instance.RespondToChatDeadSplitter.Value, "");
                }

                name = name.Trim();

                OnChatMessageReceived?.Invoke(name, split[1]);
            }
        }
    }
}
