using Newtonsoft.Json;
using System;
using System.Text;

namespace server
{
    internal class Repository
    {
        private readonly string fileName;
        private double minPrice;
        List<Component> componentList;


        public Repository()
        {
            fileName = "components.json";
            componentList = new List<Component>();
            OutOfFile();
            GetMinPrice();
        }
        Component this[string name]
        {
            get
            {
                foreach (var component in componentList)
                {
                    if (component.Name == name)
                        return component;
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        private void GetMinPrice()
        {
            minPrice = int.MaxValue;
            foreach (Component component in componentList)
            {
                if (minPrice > component.Price)
                {
                    minPrice = component.Price;
                }
            }
        }
        private void OutOfFile()
        {
            if (File.Exists(fileName))
            {
                using (StreamReader stream = new StreamReader(fileName, true))
                {
                    string json = stream.ReadToEnd();
                    if (json.Length != 0)
                    {
                        componentList = JsonConvert.DeserializeObject<List<Component>>(json);
                    }
                }
            }
        }
        public byte[] ReturnReply(DateTime userDate)
        {
            string reply = String.Empty;

            foreach (Component component in componentList)
            {
                if (component.DeliveryDate == userDate &&
                    component.Price != minPrice)
                {
                    reply += component.ToString() + "\n";
                }
            }
            return Encoding.UTF8.GetBytes(reply);
        }
        private void InFile()
        {
            using (StreamWriter stream = new StreamWriter(fileName))
            {
                string json = JsonConvert.SerializeObject(componentList);
                stream.Write(json);
            }
        }

        public void AddComponent(Component component)
        {
            componentList.Add(component);
            InFile();
            GetMinPrice();
        }

        public void DeleteComponent(Component component)
        {
            componentList.Remove(component);
            InFile();
            GetMinPrice();
        }

        public void EditComponent(Component component) 
        {
            componentList.Remove(component);

            InFile();
            GetMinPrice();
        }
    }
}
