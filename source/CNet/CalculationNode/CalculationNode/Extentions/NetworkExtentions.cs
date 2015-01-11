using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CalculationNode.Extentions
{
	public static class NetworkExtentions
	{
		public static IPAddress GetLocalIP
		{
			get
			{
				var host = Dns.GetHostEntry(Dns.GetHostName());
				return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
			}
		}

		// Get random unused port number
		public static int GetUnusedPort()
		{
			var current = Constants.DefaultRicartServerPort + 1;
			while (true)
			{
				if (CheckPortAvaliability(++current))
					return current;
			}
		}

		public static bool CheckPortAvaliability(int port)
		{
			var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
			return tcpConnInfoArray.All(tcpi => tcpi.LocalEndPoint.Port != port);
		}

		public static Uri TryBuildServerUri(string address)
		{
			if (address.Contains(":") && !address.Contains("//"))
				return new UriBuilder(
					Uri.UriSchemeHttp,
					address.Split(':')[0],
					Int32.Parse(address.Split(':')[1]),
					Constants.DefaultRelativePath)
					.Uri;
			else
				return new Uri(address);
		}

		public static Uri BuildServerUri(string ip, int port, string relativePath)
		{
			return new UriBuilder(Uri.UriSchemeHttp, ip, port, relativePath).Uri; ;
		}
	}
}
