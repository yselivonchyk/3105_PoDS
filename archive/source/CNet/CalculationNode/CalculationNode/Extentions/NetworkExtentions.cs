using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CalculationNode.Extentions
{
	public static class NetworkExtentions
	{
		public static IPAddress GetLocalIP
		{
			get
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
			bool isAvailable = true;
			IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
			foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
			{
				if (tcpi.LocalEndPoint.Port == port)
				{
					isAvailable = false;
					break;
				}
			}
			return isAvailable;
		}

		public static Uri BuildServerUri(string ip, int port, string relativePath)
		{
			return new UriBuilder(Uri.UriSchemeHttp, ip, port, relativePath).Uri; ;
		}
	}
}
