using System.Net.Sockets;
using System.Net;
using System.Text;
using server;
using System.Runtime.CompilerServices;
using System.Net.Http;

TcpListener tcpListener = new(IPAddress.Any, 8888);
Repository r = new();
int clientsNumber = 0;

try
{
    tcpListener.Start();
    while (true)
    {
        var tcpClient = await tcpListener.AcceptTcpClientAsync();
        _ = Task.Run(async () => await ProcessClientAsync(tcpClient));
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

async Task ProcessClientAsync(TcpClient tcpClient)
{
    NetworkStream stream = tcpClient.GetStream();
    clientsNumber++;

    Console.WriteLine($"Число активных клиентов: {clientsNumber}" +
        $"IP: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}\n" +
        $"Номер порта: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}\n" +
        $"Дескриптор сокета: а он где?\n\n");

    byte[] response = new byte[10];
    while (true)
    {
        string menuText = "  Меню\n" +
            "1 - вывести все детали\n" +
            "2 - добавить деталь\n" +
            "3 - редактировать деталь\n" +
            "4 - удалить деталь\n" +
            "5 - получить список деталей по дате поставки\n" +
            "0 - завершить работу\n";

        await stream.WriteAsync(Encoding.UTF8.GetBytes(menuText));
    }
    clientsNumber--;
    tcpClient.Close();
}

async Task<Component> GetComponentFromConsole(NetworkStream stream)
{
    List<byte> response = new();
    int bytesRead;

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите информацию о детали\n\nНазвание: "));
    while ((bytesRead = stream.ReadByte()) != '\n')
    {
        response.Add((byte)bytesRead);
    }
    string name = Encoding.UTF8.GetString(response.ToArray());
    response.Clear();

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Завод: "));
    while ((bytesRead = stream.ReadByte()) != '\n')
    {
        response.Add((byte)bytesRead);
    }
    string factoryName = Encoding.UTF8.GetString(response.ToArray());
    response.Clear();

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Цена: "));
    while ((bytesRead = stream.ReadByte()) != '\n')
    {
        response.Add((byte)bytesRead);
    }
    double price = double.Parse(Encoding.UTF8.GetString(response.ToArray()));
    response.Clear();

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Дата поставки: "));
    while ((bytesRead = stream.ReadByte()) != '\n')
    {
        response.Add((byte)bytesRead);
    }
    DateOnly deliveryDate = DateOnly.Parse(Encoding.UTF8.GetString(response.ToArray()));
    response.Clear();

    return new Component(name, factoryName, price, deliveryDate);
}

async Task GetComponents(NetworkStream stream)
{
    byte[] response = new byte[10];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите дату поставки: "));
    stream.Read(response, 0, 10);

    string stringResponse = Encoding.UTF8.GetString(response.ToArray());

    await stream.WriteAsync(r.ReturnReply(Convert.ToDateTime(stringResponse)));
}
