using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Samples.XmlRpc;
using System.Net.NetworkInformation;
using System.Linq;

namespace CalculationNode
{
    class Program
    {
		private static int defaultServerPort = 30001;
        static void Main(string[] args)
        {
            // Prepare params
            var localIP = LocalIPAddress();
			var fellowIpAddress = args.Length > 0 ? IPAddress.Parse(args[0]) : LocalIPAddress();
			var localPort = CheckPortAvaliability(defaultServerPort) ? defaultServerPort : GetFreePort();

			Console.WriteLine("port: " + localPort);

            // Build server URL address
            var baseAddress = new UriBuilder(Uri.UriSchemeHttp, localIP.ToString(), localPort, "/PoDS/").Uri;
            // Set up WCF endpoint
			var tokenRingAddress = new Uri(baseAddress, "./tokenring");
            ServiceHost serviceHost = new ServiceHost(typeof(TokenRingCalculator));
            var epXmlRpc = serviceHost.AddServiceEndpoint(typeof(ICalculator), 
				new WebHttpBinding(WebHttpSecurityMode.None), tokenRingAddress);
            epXmlRpc.Behaviors.Add(new XmlRpcEndpointBehavior());
            // Start the server
            serviceHost.Open();
            Console.WriteLine("Token ring listning at {0}", epXmlRpc.ListenUri);
			
			NotifyFellowServer(fellowIpAddress, tokenRingAddress.ToString());

            Console.Write("Press ENTER to quit");
            Console.ReadLine();

            serviceHost.Close();
        }

		private static void NotifyFellowServer(IPAddress fellowServerIP, string localAddress)
		{
			var serverAddress = (new UriBuilder(Uri.UriSchemeHttp, fellowServerIP.ToString(), defaultServerPort, "/PoDS/tokenring")
				.Uri).ToString();
			// Set up client channel factory
			PeersData.Add(serverAddress);

			var calculator = PeersData.GetChannel(serverAddress);
			var fellows = calculator.Join(localAddress).ToList().Where(x => !x.Equals(serverAddress));

			foreach(var fellow in fellows)
			{
				PeersData.Add(fellow);
				var fellowChannel = PeersData.GetChannel(fellow);
				var response = fellowChannel.Join(localAddress);
				ConsoleExtentions.Log(String.Format("Got response with {0} items", response.Length));
				//response.ToList().ForEach(x => Console.WriteLine("{0} - {1}", fellow, x));
			}
		}

        public static IPAddress LocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

		public static int GetFreePort()
		{
			var current = defaultServerPort + 1;
			while (true)
			{
				if (CheckPortAvaliability(++current))
					return current;
			}
		}

		public static bool CheckPortAvaliability(int port)
		{
			bool isAvailable = true;
			IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
			foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
			{
				if (tcpi.LocalEndPoint.Port==port)
				{
					isAvailable = false;
					break;
				}
			}
			return isAvailable;
		}
    }
}
