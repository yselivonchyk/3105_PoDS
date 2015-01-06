using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedCalculator
{
    class Program
    {
        public static TimeSpan maxDuration = new TimeSpan(0, 0, 20); //calculation duration 20 seconds


        static void Main(string[] args)
        {
            Client client = new Client();
            Server server = new Server();
            Thread thread1 = new Thread(new ThreadStart(client.run));
            Thread thread2 = new Thread(new ThreadStart(server.run));
            thread1.Start();
            thread2.Start();
        }
    }
}
