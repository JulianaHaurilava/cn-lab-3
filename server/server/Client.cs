using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class Client
    {
        int number;
        NetworkStream stream;
        Repository r;

        public Client(int number, NetworkStream stream)
        {
            this.number = number;
            this.stream = stream;
        }
    }
}
