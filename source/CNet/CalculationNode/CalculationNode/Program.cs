using System;
using System.Threading;
using CalculationNode.Extentions;
using CalculationNode.RicartAgrawala;

namespace CalculationNode
{
    class Program
    {
	    private static RicartAgrawalaClient ricartAgrawalaClient;
        static void Main(string[] args)
        {
			ConsoleExtentions.Regresh();
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
	        Console.Title = baseAddress.ToString();
			ricartAgrawalaClient = new RicartAgrawalaClient(baseAddress);
			ricartAgrawalaClient.Join(fellowAddress);

	        RunInputLoop();

	        Console.WriteLine("Aplication terminatesion 1 second");
			Thread.Sleep(1000);
        }

	    private static void RunInputLoop()
	    {
		    while (true)
		    {
				switch (Console.ReadKey().Key)
				{
					case ConsoleKey.D1:
						Console.WriteLine("\n\rEnter url to Join:");
						ricartAgrawalaClient.Join(
							NetworkExtentions.TryBuildServerUri(Console.ReadLine()));
						break;
					case ConsoleKey.D2:
						ricartAgrawalaClient.SingOff();
						break;
					case ConsoleKey.D3:
						var seed = (new Random().Next(1, 10));
						ConsoleExtentions.Log("Seed Data: " + seed);
						ricartAgrawalaClient.Start(seed);
						break;
					case ConsoleKey.D8:
						ConsoleExtentions.Regresh();
						break;
					case ConsoleKey.D9:
						PeersData.ListPeers();
						break;
					case ConsoleKey.Escape:
						return;
					default:
						Console.WriteLine("\r\nUnknown command");
						break;
				}    
		    }
	    }
    }
}
