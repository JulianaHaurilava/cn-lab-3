using System.Net.Sockets;
using System.Net;
using System.Text;
using server;

TcpListener tcpListener = new(IPAddress.Any, 8888);
Repository r = new();

//Component component = new("Деталь_5", "Завод_5", 5000, new DateTime(2002, 02, 02));
//r.Add(component);
int clientsNumber = 0;

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
    clientsNumber++;
    NetworkStream stream = tcpClient.GetStream();

    // буфер для входящих данных
    byte[] response = new byte[11];
    while (true)
    {
        stream.Read(response, 0, 10);

        string stringResponse = Encoding.UTF8.GetString(response.ToArray());

        if (stringResponse == "<stop>") break;

        Console.WriteLine($"Число активных клиентов: {clientsNumber}" +
            $"IP: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}\n" +
            $"Номер порта: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}\n" +
            $"Дескриптор сокета: а он где?\n\n");

        await stream.WriteAsync(r.ReturnReply(Convert.ToDateTime(stringResponse)));
        response = new byte[11];
    }
    clientsNumber--;
    tcpClient.Close();
}
