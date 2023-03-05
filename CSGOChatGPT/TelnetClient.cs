using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace CSGOChatGPT.Networking {

    public class TelnetClient : IDisposable {
        private string host;
        private int port;

        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread readThread;

        public Action<string> OnResponseReceived { get; set; }

        public TelnetClient(string host, int port) {
            this.host = host;
            this.port = port;
        }

        public void Connect() {
            try {
                Console.WriteLine("Connecting to " + host + ":" + port);

                // Connect to the telnet server
                client = new TcpClient(host, port);

                // Get the network stream for sending and receiving data
                stream = client.GetStream();

                // Create a stream reader and writer for reading and writing data
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                // Start a thread to read responses from the server
                readThread = new Thread(new ThreadStart(ReadResponse));
                readThread.Start();
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void SendCommand(string command) {
            try {
                // Send a command to the telnet server
                writer.WriteLine(command);
                writer.Flush();
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ReadResponse() {
            try {
                while (true) {
                    // Read the response from the telnet server
                    string response = reader.ReadLine();

                    // Raise the OnResponseReceived event with the response
                    if (OnResponseReceived != null) {
                        OnResponseReceived(response);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void Disconnect() {
            // Close the streams and the client
            reader.Close();
            writer.Close();
            stream.Close();
            client.Close();
        }

        public void Dispose() {
            Disconnect();
            reader.Dispose();
            writer.Dispose();
            stream.Dispose();
            client.Dispose();
        }
    }
}
