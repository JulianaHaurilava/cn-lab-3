using System.Net.Sockets;
using System.Text;

namespace client
{
    internal class Client
    {
        private NetworkStream? stream;
        readonly string menuText = "  Меню\n"
      + "1 - получить информацию обо всех деталях\n"
      + "2 - добавить деталь\n"
      + "3 - удалить деталь\n"
      + "4 - получить список деталей по дате поставки\n"
      + "0 - завершить работу";

        public async Task StartAsync()
        {
            using TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 8888);
            stream = tcpClient.GetStream();
            await ShowMainMenuAsync();
        }
        private async Task ShowMainMenuAsync()
        {
            while (true)
            {
                Console.WriteLine(menuText);

                string message = Console.ReadLine();
                int choice = int.Parse(message);

                await SendMessageAsync(message);

                switch (choice)
                {
                    case 1:
                        string stringResponse = await ReceiveMessageAsync(1024);
                        Console.WriteLine(stringResponse);
                        break;
                    case 2:
                        await AddComponentAsync();
                        break;
                    case 3:
                        await RemoveComponent();
                        break;
                    case 4:
                        await GetComponentByDate();
                        break;
                    case 0:
                        return;
                }
            }
        }
        async Task GetComponentFromConsoleAsync()
        {
            Console.Write("Введите информацию о детали\n\nНазвание: ");
            await SendMessageAsync(Console.ReadLine());

            Console.Write("Завод: ");
            await SendMessageAsync(Console.ReadLine());

            Console.Write("Цена:");
            await SendMessageAsync(Console.ReadLine());

            Console.Write("Дата поставки:");
            await SendMessageAsync(Console.ReadLine());
        }
        async Task AddComponentAsync()
        {
            await GetComponentFromConsoleAsync();
            string response = await ReceiveMessageAsync(500);
            Console.WriteLine(response);
        }
        async Task RemoveComponent()
        {
            Console.Write("Введите название детали, которую хотите удалить\n\nНазвание: ");
            await SendMessageAsync(Console.ReadLine());

            string response = await ReceiveMessageAsync(500);
            Console.WriteLine(response);
        }
        async Task GetComponentByDate()
        {
            Console.Write("Введите дату поставки: ");
            await SendMessageAsync(Console.ReadLine());

            string response = await ReceiveMessageAsync(500);
            Console.WriteLine(response);
        }
        private async Task SendMessageAsync(string message)
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        }
        private async Task<string> ReceiveMessageAsync(int maxByteNumber)
        {
            byte[] response = new byte[maxByteNumber];
            await stream.ReadAsync(response, 0, maxByteNumber);
            string stringResponse = Encoding.UTF8.GetString(response).TrimEnd('\0');

            return stringResponse;
        }
    }
}
