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
        public Component this[string name]
        {
            get
            {
                foreach (var component in componentList)
                {
                    if (component.Name == name)
                        return component;
                }
                return new Component();
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
        public byte[] ReturnReply(DateOnly userDate)
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

        public void EditComponent(Component component, Component newComponent) 
        {
            componentList.Remove(component);
            componentList.Add(newComponent);
            InFile();
            GetMinPrice();
        }

        public string GetAllComponents()
        {
            string allDetails = String.Empty;
            foreach(Component component in componentList)
            {
                allDetails += component.ToString() + "\n";
            }
            return allDetails;
        }
    }
}
