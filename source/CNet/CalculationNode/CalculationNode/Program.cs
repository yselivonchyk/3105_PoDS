using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Samples.XmlRpc;
using System.Net.NetworkInformation;

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
			var localPort = CheckPortAvaliability(defaultServerPort) ? defaultServerPort : -1;

            // Build server URL address
            var baseAddress = new UriBuilder(Uri.UriSchemeHttp, localIP.ToString(), localPort, "/PoDS/").Uri;
            // Set up WCF endpoint
            ServiceHost serviceHost = new ServiceHost(typeof(TokenRingCalculator));
            var epXmlRpc = serviceHost.AddServiceEndpoint(typeof(ICalculator), 
				new WebHttpBinding(WebHttpSecurityMode.None), new Uri(baseAddress, "./tokenring"));
            epXmlRpc.Behaviors.Add(new XmlRpcEndpointBehavior());
            // Start the server
            serviceHost.Open();
            Console.WriteLine("Token ring listning at {0}", epXmlRpc.ListenUri);

			if (args.Length > 0 || localPort != defaultServerPort)
			{
				NotifyFellowServer(args);
			}

            Console.Write("Press ENTER to quit");
            Console.ReadLine();

            serviceHost.Close();
        }

		private static void NotifyFellowServer(string[] args)
		{
			//Uri serverAddress = new UriBuilder(address).Uri;
			//// Set up client channel factory
			//ChannelFactory<ICalculator> bloggerAPIFactory = new ChannelFactory<ICalculator>(
			//	new WebHttpBinding(WebHttpSecurityMode.None), new EndpointAddress(serverAddress));
			//bloggerAPIFactory.Endpoint.Behaviors.Add(new XmlRpcEndpointBehavior());

			//// Check that it works
			//ICalculator bloggerAPI = bloggerAPIFactory.CreateChannel();

			//Console.WriteLine("Smoke test: " + bloggerAPI.Join("smoke")[0]);
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
