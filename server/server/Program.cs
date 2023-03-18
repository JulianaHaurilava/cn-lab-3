using System.Net.Sockets;
using System.Net;
using System.Text;
using server;

TcpListener tcpListener = new(IPAddress.Any, 8888);
Repository r = new();

try
{
    tcpListener.Start();
    while (true)
    {
        // получаем подключение в виде TcpClient
        var tcpClient = await tcpListener.AcceptTcpClientAsync();

        // создаем новую задачу для обслуживания нового клиента
        Task.Run(async () => await ProcessClientAsync(tcpClient));

    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

async Task ProcessClientAsync(TcpClient tcpClient)
{
    NetworkStream stream = tcpClient.GetStream();

    // буфер для входящих данных
    byte[] response = new byte[11];
    while (true)
    {
        stream.Read(response, 0, 10);

        string stringResponse = Encoding.UTF8.GetString(response.ToArray());

        if (stringResponse == "<stop>") break;

        Console.WriteLine($"IP: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}" +
            $"Номер порта: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}" +
            $"Дескриптор сокета: а он где?");

        await stream.WriteAsync(r.ReturnReply(Convert.ToDateTime(stringResponse)));
        response = new byte[11];
    }
    tcpClient.Close();
}
