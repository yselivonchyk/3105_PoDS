using System;
using CalculationNode.Extentions;
using CalculationNode.RicartAgrawala;

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
	        var fellowAddress = args.Length > 0
		        ? new Uri(args[0])
		        : NetworkExtentions.BuildServerUri(
			        NetworkExtentions.GetLocalIP.ToString(),
			        Constants.DefaultRicartServerPort,
			        Constants.DefaultRelativePath);

			// Build server URL address
	        var baseAddress = NetworkExtentions.BuildServerUri(localIP.ToString(), localPort, Constants.DefaultRelativePath);

	        var ricardClient = new RicartAgrawalaClient(baseAddress);
			ricardClient.Join(fellowAddress);

			// TODO: Implement infinite loop for user interractions
			// TODO: Implement EventGenerator to be start on user request

	        Console.ReadLine();
        }
    }
}
