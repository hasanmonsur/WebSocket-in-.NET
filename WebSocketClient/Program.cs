using System.Net.WebSockets;
using System.Text;

namespace WebSocketClient
{
    internal class Program
    {

        static async Task Main()
        {
            Uri serverUri = new Uri("ws://localhost:5284/ws");

            using var client = new ClientWebSocket();
            Console.WriteLine("Connecting to WebSocket server...");
            await client.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected!");

            _ = Task.Run(() => ReceiveMessages(client));

            while (client.State == WebSocketState.Open)
            {
                Console.Write("Enter message: ");
                string message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(message)) continue;

                await SendMessage(client, message);
                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            Console.WriteLine("Disconnected.");
        }

        static async Task SendMessage(ClientWebSocket client, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static async Task ReceiveMessages(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Server closed connection.");
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"\nReceived from server: {message}");
                Console.Write("Enter message: ");
            }
        }
    }
}
