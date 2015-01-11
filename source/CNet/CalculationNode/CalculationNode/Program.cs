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

			ricartAgrawalaClient = new RicartAgrawalaClient(baseAddress);
			ricartAgrawalaClient.Join(fellowAddress);

	        RunInputLoop();

	        Console.WriteLine("Aplication terminatesion 1 second");
			Thread.Sleep(1000);
        }

	    private static void RunInputLoop()
	    {
			Console.WriteLine("\nPress key to indicate desired operation:");
			Console.WriteLine("Enter - start command");
			Console.WriteLine("J - Join command");
			Console.WriteLine("S - Sign off command");
			Console.WriteLine("Esc - exit");
		    while (true)
		    {
				switch (Console.ReadKey().Key)
				{
					case ConsoleKey.Enter:
						ricartAgrawalaClient.Start(5);
						break;
					case ConsoleKey.S:
						ricartAgrawalaClient.SingOff();
						break;
					case ConsoleKey.J:
						Console.WriteLine("\n\rEnter url to Join:");
						ricartAgrawalaClient.Join(
							NetworkExtentions.TryBuildServerUri(Console.ReadLine()));
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
