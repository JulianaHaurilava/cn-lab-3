using System.Net.Sockets;
using System.Text;

using TcpClient tcpClient = new TcpClient();
await tcpClient.ConnectAsync("127.0.0.1", 8888);
var stream = tcpClient.GetStream();

while (true)
{
    byte[] response = new byte[3000];
    Console.Write("Введите дату поставки: ");
    string message = Console.ReadLine();

    //if (message == "<stop>")
    //{
    //    tcpClient.Close();
    //    break;
    //}
    await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
    stream.Read(response, 0, 3000);

    string stringResponse = Encoding.UTF8.GetString(response.ToArray());
    Console.WriteLine(stringResponse);
    response = new byte[11];
}