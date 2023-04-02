using System.ComponentModel;
using System.Net.Sockets;
using System.Text;

string menuText = "  Меню\n"
      + "1 - получить информацию обо всех деталях\n"
      + "2 - получить список деталей по дате поставки\n"
      + "0 - завершить работу";
using TcpClient tcpClient = new TcpClient();
await tcpClient.ConnectAsync("127.0.0.1", 8888);
var stream = tcpClient.GetStream();

try
{
    while (true)
    {
        Console.WriteLine(menuText);

        string message = Console.ReadLine();
        int choice = int.Parse(message);
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));

        switch (choice)
        {
            case 1:
                byte[] response = new byte[1024];
                await stream.ReadAsync(response, 0, 1024);
                string stringResponse = Encoding.UTF8.GetString(response).TrimEnd('\0');
                Console.WriteLine(stringResponse);
                break;
            case 2:
                await GetComponentFromConsoleAsync(stream);
                break;
            case 3:
                await EditComponentAsync(stream);
                break;
            case 4:
                break;
            case 5:

                break;
            case 0:
                return;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

async Task GetComponentFromConsoleAsync(NetworkStream stream)
{
    Console.Write("Введите информацию о детали\n\nНазвание: ");
    await stream.WriteAsync(Encoding.UTF8.GetBytes(Console.ReadLine()));

    Console.Write("Завод: ");
    await stream.WriteAsync(Encoding.UTF8.GetBytes(Console.ReadLine()));

    Console.Write("Цена:");
    await stream.WriteAsync(Encoding.UTF8.GetBytes(Console.ReadLine()));

    Console.Write("Дата поставки:");
    await stream.WriteAsync(Encoding.UTF8.GetBytes(Console.ReadLine()));
}

async Task EditComponentAsync(NetworkStream stream)
{
    Console.Write("Введите название детали: ");
    await stream.WriteAsync(Encoding.UTF8.GetBytes(Console.ReadLine()));

    await GetComponentFromConsoleAsync(stream);
}
