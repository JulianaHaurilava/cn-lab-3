using System.Text.Json.Serialization;

namespace server
{
    internal class Component
    {
        public string FactoryName { get; set; }

        public double Price { get; set; }

        public DateTime DeliveryDate { get; set; } 

        public Component()
        {
            FactoryName = String.Empty;
            Price = 0;
            DeliveryDate = DateTime.MinValue;
        }

        [JsonConstructor]
        public Component(string factoryName, double price, DateTime deliveryDate)
        {
            FactoryName = factoryName;
            Price = price;
            DeliveryDate = deliveryDate;
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
