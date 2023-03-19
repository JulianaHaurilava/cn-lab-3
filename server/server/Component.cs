
using System.Text.Json.Serialization;

namespace server
{
    internal class Component
    {
        private string v;

        public string Name { get; set; }
        public string FactoryName { get; set; }
        public double Price { get; set; }
        public DateTime DeliveryDate { get; set; } 

        public Component()
        {
            Name = String.Empty;
            FactoryName = String.Empty;
            Price = 0;
            DeliveryDate = DateTime.MinValue;
        }

        [JsonConstructor]
        public Component(string name, string factoryName, double price, DateTime deliveryDate)
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

        //public JObject GetJson()
        //{
        //    JObject jClient = new JObject();
        //    jClient["FactoryName"] = FactoryName;
        //    jClient["Price"] = Price;
        //    jClient["DeliveryDate"] = DeliveryDate;
        //    return jClient;
        //}

    }
}
