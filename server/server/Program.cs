using System.Net.Sockets;
using System.Net;
using System.Text;
using server;

TcpListener tcpListener = new(IPAddress.Any, 8888);
Repository r = new();
int clientsNumber = 0;
string menuText = "  Меню\n" +
            "1 - вывести все детали\n" +
            "2 - добавить деталь\n" +
            "3 - редактировать информацию о детали\n" +
            "4 - удалить деталь\n" +
            "5 - получить список деталей по дате поставки\n" +
            "0 - завершить работу\n";

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
        await stream.WriteAsync(Encoding.UTF8.GetBytes(menuText));
        stream.Read(response, 0, 1);
        int choice  = int.Parse(Encoding.UTF8.GetString(response.ToArray()));

        switch (choice)
        {
            case 1:
                await stream.WriteAsync(Encoding.UTF8.GetBytes(r.GetAllComponents()));
                break;
            case 2:
                Component newComponent = await GetComponentFromConsole(stream);
                r.AddComponent(newComponent);
                break;
            case 3:
                await EditComponent(stream);
                break;
            case 4:
                await RemoveComponent(stream);
                break;
            case 5:
                await GetComponents(stream);
                break;
            case 0:
                clientsNumber--;
                tcpClient.Close();
                break;
            default:
                break;
        }
    }
}

async Task<Component> GetComponentFromConsole(NetworkStream stream)
{
    byte[] response = new byte[30];
    int bytesRead;

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите информацию о детали\n\nНазвание: "));
    stream.Read(response, 0, 30);
    string name = Encoding.UTF8.GetString(response.ToArray());
    response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Завод: "));
    stream.Read(response, 0, 30);
    string factoryName = Encoding.UTF8.GetString(response.ToArray());
    response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Цена: "));
    stream.Read(response, 0, 30);
    double price = double.Parse(Encoding.UTF8.GetString(response.ToArray()));
    response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Дата поставки: "));
    stream.Read(response, 0, 30);
    DateOnly deliveryDate = DateOnly.Parse(Encoding.UTF8.GetString(response.ToArray()));

    return new Component(name, factoryName, price, deliveryDate);
}

async Task GetComponents(NetworkStream stream)
{
    byte[] response = new byte[10];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите дату поставки: "));
    stream.Read(response, 0, 10);

    string stringResponse = Encoding.UTF8.GetString(response.ToArray());

    await stream.WriteAsync(r.ReturnReply(DateOnly.Parse(stringResponse)));
}

async Task EditComponent(NetworkStream stream)
{
    List<byte> response = new();
    int bytesRead;

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите название детали: "));
    while ((bytesRead = stream.ReadByte()) != '\n')
    {
        response.Add((byte)bytesRead);
    }
    string name = Encoding.UTF8.GetString(response.ToArray());
    response.Clear();

    Component newComponent = await GetComponentFromConsole(stream);

    r.EditComponent(r[name], newComponent);
}

async Task RemoveComponent(NetworkStream stream)
{
    List<byte> response = new();
    int bytesRead;

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите название детали: "));
    while ((bytesRead = stream.ReadByte()) != '\n')
    {
        response.Add((byte)bytesRead);
    }
    string name = Encoding.UTF8.GetString(response.ToArray());
    response.Clear();

    r.DeleteComponent(r[name]);
}