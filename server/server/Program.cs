using System.Net.Sockets;
using System.Net;
using server;

TcpListener tcpListener = new(IPAddress.Any, 8888);
Console.WriteLine("Ожидание подключения...\n");

Repository r = new();
int clientsNumber = 0;

try
{
    tcpListener.Start();
    while (true)
    {
        var tcpClient = await tcpListener.AcceptTcpClientAsync();
        _ = Task.Run(async () => await WorkWithClient(tcpClient));
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

async Task WorkWithClient(TcpClient tcpClient)
{
    NetworkStream stream = tcpClient.GetStream();
    ClientSession client = new(++clientsNumber, stream, r);

    Console.WriteLine($"Установлено подключение с новым клиентом!\n" +
        $"Число активных клиентов: {clientsNumber}\n" +
        $"IP: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}\n" +
        $"Номер порта: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}\n" +
        $"Дескриптор сокета: {tcpClient.Client.Handle}\n");

    await client.ConnectAsync();
    tcpClient.Close();
    clientsNumber--;
    Console.WriteLine($"Соединение с клиентом разорвано!\n" +
        $"Число активных клиентов: {clientsNumber}\n");
}

