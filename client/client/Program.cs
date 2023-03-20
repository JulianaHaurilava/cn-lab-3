using System.Net.Sockets;
using System.Text;

using TcpClient tcpClient = new TcpClient();
await tcpClient.ConnectAsync("127.0.0.1", 8888);
var stream = tcpClient.GetStream();

try
{
    while (true)
    {
        byte[] response = new byte[3000];
        stream.Read(response, 0, 3000);

        string stringResponse = Encoding.UTF8.GetString(response).TrimEnd('\0');
        Console.WriteLine(stringResponse);

        string message = Console.ReadLine();
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}