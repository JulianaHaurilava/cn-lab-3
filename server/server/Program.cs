using System.Net.Sockets;
using System.Net;
using System.Text;
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
    Client client = new Client(++clientsNumber, stream);

    Console.WriteLine($"Установлено подключение с новым клиентом!\n" +
        $"Число активных клиентов: {clientsNumber}\n" +
        $"IP: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}\n" +
        $"Номер порта: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}\n" +
        $"Дескриптор сокета: {tcpClient.Client.Handle}\n");

    await GetMenu(stream, clientNumber);
    clientsNumber--;
    tcpClient.Close();
}

async Task GetMenu(NetworkStream stream, int clientNumber)
{
    string menuText = "  Меню\n" +
            "1 - информацию обо всех деталях\n" +
            "2 - добавить деталь\n" +
            "3 - редактировать информацию о детали\n" +
            "4 - удалить деталь\n" +
            "5 - получить список деталей по дате поставки\n" +
            "0 - завершить работу";
    byte[] response = new byte[10];
    while (true)
    {
        await SendMessageAsync(stream, clientNumber, menuText);
        stream.Read(response, 0, 1);
        int choice = int.Parse(Encoding.UTF8.GetString(response));

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
                return;
        }
        stream.Flush();
    }
}

async Task<Component> GetComponentFromConsole(NetworkStream stream)
{
    byte[] response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите информацию о детали\n\nНазвание: "));
    stream.Read(response, 0, 30);
    string name = Encoding.UTF8.GetString(response).TrimEnd('\0');
    response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Завод: "));
    stream.Read(response, 0, 30);
    string factoryName = Encoding.UTF8.GetString(response).TrimEnd('\0');
    response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Цена: "));
    stream.Read(response, 0, 30);
    double price = double.Parse(Encoding.UTF8.GetString(response).TrimEnd('\0'));
    response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Дата поставки: "));
    stream.Read(response, 0, 30);
    DateOnly deliveryDate = DateOnly.Parse(Encoding.UTF8.GetString(response).TrimEnd('\0'));

    return new Component(name, factoryName, price, deliveryDate);
}

async Task GetComponents(NetworkStream stream)
{
    byte[] response = new byte[10];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите дату поставки: "));
    stream.Read(response, 0, 10);

    string stringResponse = Encoding.UTF8.GetString(response);

    await stream.WriteAsync(r.ReturnReply(DateOnly.Parse(stringResponse)));
}

async Task EditComponent(NetworkStream stream)
{
    byte[] response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите название детали: "));
    stream.Read(response, 0, 30);
    string name = Encoding.UTF8.GetString(response);

    Component newComponent = await GetComponentFromConsole(stream);

    r.EditComponent(r[name], newComponent);
}

async Task RemoveComponent(NetworkStream stream)
{
    byte[] response = new byte[30];

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Введите название детали: "));
    stream.Read(response, 0, 30);
    string name = Encoding.UTF8.GetString(response);

    r.DeleteComponent(r[name]);
}

async Task SendMessageAsync(NetworkStream stream, int clientNumber, string message)
{
    await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
    Console.WriteLine($"Клиенту с номером №{clientNumber} отправлено сообщение в {DateTime.Now}\n" +
        $"==========Сообщение==========\n\n" +
        $"{message}\n\n" +
        $"==========Конец сообщения==========\n");
}