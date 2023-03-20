using System.Net.Sockets;
using System.Text;

namespace server
{
    internal class ClientSession
    {
        private readonly int number;
        private readonly NetworkStream stream;
        private readonly Repository r;

        public ClientSession(int number, NetworkStream stream, Repository r)
        {
            this.number = number;
            this.stream = stream;
            this.r = r;
        }

        public async Task ConnectAsync()
        {
            string menuText = "  Меню\n" +
                    "1 - информацию обо всех деталях\n" +
                    "2 - добавить деталь\n" +
                    "3 - редактировать информацию о детали\n" +
                    "4 - удалить деталь\n" +
                    "5 - получить список деталей по дате поставки\n" +
                    "0 - завершить работу";
            while (true)
            {
                await SendMessageAsync(menuText);
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
                        await EditComponentAsync();
                        break;
                    case 4:
                        await RemoveComponentAsync();
                        break;
                    case 5:
                        await GetComponentsByDateAsync();
                        break;
                    case 0:
                        return;
                }
                stream.Flush();
            }
        }

        private async Task GetAllComponentsAsync()
        {
            await SendMessageAsync(r.GetAllComponents()
                + "Отправьте любое сообщение чтобы продолжить...");
            await ReceiveMessageAsync(1);
        }
        private async Task AddNewComponentAsync()
        {
            Component newComponent = await GetComponentFromConsoleAsync();
            r.AddComponent(newComponent);
        }
        private async Task EditComponentAsync()
        {
            await SendMessageAsync("Введите название детали: ");
            string name = await ReceiveMessageAsync(24);

            Component newComponent = await GetComponentFromConsoleAsync();
            r.EditComponent(r[name], newComponent);
        }
        private async Task RemoveComponentAsync()
        {
            await SendMessageAsync("Введите название детали: ");
            string name = await ReceiveMessageAsync(24);

            r.DeleteComponent(r[name]);
        }
        private async Task GetComponentsByDateAsync()
        {
            await SendMessageAsync("Введите дату поставки:");
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

        private async Task<Component> GetComponentFromConsoleAsync()
        {
            await SendMessageAsync("Введите информацию о детали\n\nНазвание:");
            string name = await ReceiveMessageAsync(24);

            await SendMessageAsync("Завод:");
            string factoryName = await ReceiveMessageAsync(24);

            await SendMessageAsync("Цена:");
            double price = double.Parse(await ReceiveMessageAsync(24));

            await SendMessageAsync("Дата поставки:");
            DateOnly deliveryDate = DateOnly.Parse(await ReceiveMessageAsync(24));

            return new Component(name, factoryName, price, deliveryDate);
        }
    }
}
