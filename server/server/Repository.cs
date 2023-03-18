﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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

        private void GetMinPrice()
        {
            minPrice = 0;
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

        //private void InFile()
        //{
        //    using (StreamWriter stream = new StreamWriter(fileName))
        //    {
        //        string json = JsonConvert.SerializeObject(componentList);
        //        stream.Write(json);
        //    }
        //}
    }
}
