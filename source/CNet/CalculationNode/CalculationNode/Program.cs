using System;
using System.Net;
using System.ServiceModel;
using CalculationNode.Extentions;
using CalculationNode.RicartAgrawala;
using Microsoft.Samples.XmlRpc;
using System.Linq;

namespace CalculationNode
{
    class Program
    {
		
        static void Main(string[] args)
        {
            // Prepare params
            var localIP = NetworkExtentions.GetLocalIP;
			var localPort = NetworkExtentions.CheckPortAvaliability(Constants.DefaultRicartServerPort)
				? Constants.DefaultRicartServerPort 
				: NetworkExtentions.GetUnusedPort();
			var fellowAddress = args.Length > 0 ? new Uri(args[0]) : NetworkExtentions.BuildServerUri(
				NetworkExtentions.GetLocalIP.ToString(),
				Constants.DefaultRicartServerPort,
				Constants.DefaultRelativePath);

			// Build server URL address
	        var baseAddress = NetworkExtentions.BuildServerUri(localIP.ToString(), localPort, Constants.DefaultRelativePath);

	        var ricardClient = new RicartAgrawalaClient(baseAddress);
			ricardClient.Join(fellowAddress);

	        Console.ReadLine();
        }

		private static void NotifyFellowServers(IPAddress fellowServerIP, string localAddress)
		{
			var serverAddress = (new UriBuilder(Uri.UriSchemeHttp, fellowServerIP.ToString(), Constants.DefaultRicartServerPort, "/PoDS/tokenring")
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


    }
}
