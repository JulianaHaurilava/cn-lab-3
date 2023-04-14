using System.Net.Sockets;
using System.Text;

namespace server
{
    internal class ServerSession
    {
        private readonly int number;
        private readonly NetworkStream stream;
        private readonly Repository r;

        public ServerSession(int number, NetworkStream stream, Repository r)
        {
            this.number = number;
            this.stream = stream;
            this.r = r;
        }

        public async Task ConnectAsync()
        {
            while (true)
            {
                int choice = int.Parse(await ReceiveMessageAsync(1));

                switch (choice)
                {
                    case 1:
                        await GetAllComponentsAsync();
                        break;
                    case 2:
                        await AddNewComponentAsync();
                        break;
                    case 3:
                        await RemoveComponentAsync();
                        break;
                    case 4:
                        await GetComponentsByDateAsync();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private async Task GetAllComponentsAsync()
        {
            await SendMessageAsync(r.GetAllComponents());
        }
        private async Task AddNewComponentAsync()
        {
            Component newComponent = await ReceiveComponent();
            bool isAdded = r.AddComponent(newComponent);
            if (isAdded)
                await SendMessageAsync("Деталь успешно добавлена!\n");
            else
                await SendMessageAsync("Деталь с таким названием уже существует.\n");
        }
        private async Task RemoveComponentAsync()
        {
            string name = await ReceiveMessageAsync(24);
            bool isDeleted = r.DeleteComponent(r[name]);

            if (isDeleted)
                await SendMessageAsync("Информация о детали удалена!\n");
            else
                await SendMessageAsync("Детали с введенным названием не существует.\n");
        }
        private async Task GetComponentsByDateAsync()
        {
            string stringResponse = await ReceiveMessageAsync(10);
            await SendMessageAsync(r.GetComponents(DateOnly.Parse(stringResponse)));
        }

        private async Task SendMessageAsync(string message)
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
            Console.WriteLine($"Клиенту с номером №{number} отправлено сообщение в {DateTime.Now}\n" +
                $"==========Сообщение==========\n\n" +
                $"{message}\n\n" +
                $"==========Конец сообщения==========\n");
        }
        private async Task<string> ReceiveMessageAsync(int maxByteNumber)
        {
            byte[] response = new byte[maxByteNumber];
            await stream.ReadAsync(response, 0, maxByteNumber);
            string stringResponse = Encoding.UTF8.GetString(response).TrimEnd('\0');

            Console.WriteLine($"Клиент №{number} прислал сообщение {DateTime.Now}\n" +
                $"==========Сообщение==========\n\n" +
                $"{stringResponse}\n\n" +
                $"==========Конец сообщения==========\n");

            return stringResponse;
        }
        private async Task<Component> ReceiveComponent()
        {
            string name = await ReceiveMessageAsync(24);
            string factoryName = await ReceiveMessageAsync(24);
            double price = double.Parse(await ReceiveMessageAsync(24));
            DateOnly deliveryDate = DateOnly.Parse(await ReceiveMessageAsync(24));

            return new Component(name, factoryName, price, deliveryDate);
        }
    }
}
