using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace DistributedCalculator
{
    class Program
    {
        public static TimeSpan maxDuration = new TimeSpan(0, 0, 20); //calculation duration 20 seconds

        static void Main(string[] args)
        {
            int port = findFreePort();
            Client client = new Client(port);
            Server server = new Server(port);
            server.run();
            client.run();
        }

        private static int findFreePort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
