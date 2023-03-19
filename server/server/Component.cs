
using System.Text.Json.Serialization;

namespace server
{
    internal class Component
    {
        public string Name { get; set; }
        public string FactoryName { get; set; }
        public double Price { get; set; }
        public DateOnly DeliveryDate { get; set; } 

        public Component()
        {
            Name = String.Empty;
            FactoryName = String.Empty;
            Price = 0;
            DeliveryDate = DateOnly.MinValue;
        }

        [JsonConstructor]
        public Component(string name, string factoryName, double price, DateOnly deliveryDate)
        {
            Name = name;
            FactoryName = factoryName;
            Price = price;
            DeliveryDate = deliveryDate;
        }

        public override string ToString()
        {
            return "Название: " + Name + "\n" +
                "Завод: " + FactoryName + "\n" +
                "Цена: " + Price + "\n" +
                "Дата поставки: " + DeliveryDate + "\n";
        }
    }
}
