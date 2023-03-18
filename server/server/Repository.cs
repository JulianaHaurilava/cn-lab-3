using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class Repository
    {
        protected readonly string fileName;
        List<Component> componentList;

        public Repository()
        {
            componentList = new List<Component>();
        }

        public List<Component> ReturnComponents(DateTime userDate)
        {
            List<Component> neededComponents = new();

        }

        public Repository()
        {
            fileName = "clients.json";
            AllClients = new ObservableCollection<Client>();

            OutOfFile();
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
                        AllClients = JsonConvert.DeserializeObject<ObservableCollection<Client>>(json);
                    }
                }
            }
        }
        private void InFile()
        {
            JArray clientsArray = new JArray();
            foreach (Client client in AllClients)
            {
                clientsArray.Add(client.GetJson());
            }

            using (StreamWriter stream = new StreamWriter(fileName))
            {
                string json = JsonConvert.SerializeObject(clientsArray, Formatting.Indented);
                stream.Write(json);
            }
        }
    }
}
