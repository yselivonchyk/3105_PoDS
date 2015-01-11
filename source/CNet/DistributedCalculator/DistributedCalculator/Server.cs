using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace DistributedCalculator
{
	public interface BasicOperations
	{

		[XmlRpcMethod("Node.join")] //return the nodes in the network
		Object[] join(String thisHostInfo);

		[XmlRpcMethod("Node.signOff")]
		bool signOff(String leavingMachine);

		[XmlRpcMethod("Node.start")] //start a round of calculation
		bool start(int initValue);

		[XmlRpcMethod("Node.doCalculation")] //execute operation
		bool doCalculation(String operation, String value);

		[XmlRpcMethod("Node.finalizeSession")]//Counts finished machines
		bool finalizeSession();
	}

	class Server : MarshalByRefObject, BasicOperations
	{
		private static int port;
		public static String thisHostInfo;
		public static int finishedSessions = 0;
		private int processValue = 0;

		public Server()
		{
			port = findFreePort();
			thisHostInfo = getThisHostInfo();
		}

		public void run()
		{
			IDictionary props = new Hashtable();
			props["name"] = "MyHttpChannel";
			props["port"] = port;
			HttpChannel channel = new HttpChannel(
			  props,
			  null,
			  new XmlRpcServerFormatterSinkProvider()
		   );
			ChannelServices.RegisterChannel(channel, false);
			RemotingConfiguration.RegisterWellKnownServiceType(
			  typeof(Server),
			  "xmlrpc",
			  WellKnownObjectMode.Singleton);
		}

		public bool start(int initValue)
		{
			Console.WriteLine("Starting calculations!");
			if (Client.algorithm == 0) Console.WriteLine("TokenRing algorithm was chosen!");
			else Console.WriteLine("Ricart and Agrawala algorithm was chosen!");
			return true;
		}

		public bool doCalculation(String operation, String value)
		{
			int intValue = Int32.Parse(value);
			switch (operation)
			{
				case "sum":
					processValue += intValue;
					break;
				case "div":
					processValue /= intValue;
					break;
				case "sub":
					processValue -= intValue;
					break;
				case "mul":
					processValue *= intValue;
					break;
				default:
					Console.WriteLine("Unknown operation in doCalculation!");
					return false;
			}
			return true;
		}

		public Object[] join(String IpAndPort)
		{
			if (Client.Nodes.Add(IpAndPort))
			{
				Console.WriteLine("New node connected!\nNode info: " + IpAndPort + "\n");
			}
			return Client.Nodes.ToArray();
		}

		public bool signOff(String leavingMachine)
		{
			Console.WriteLine("Machine " + leavingMachine + " left network!");
			return Client.Nodes.Remove(leavingMachine);
		}

		public bool finalizeSession()
		{
			finishedSessions++;
			Console.WriteLine("FINISHED RECEIVED! Machines: " + finishedSessions);
			if (finishedSessions == Client.Nodes.Count)
			{
				Console.WriteLine("Session Ended! FINAL RESULT: "
						+ processValue);
			}
			return true;
		}

		private static String getThisHostInfo()
		{
			String ownIP = "127.0.0.1";
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			//get the local IP address
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					ownIP = ip.ToString();
				}
			}
			return ownIP + ":" + Convert.ToString(port);
		}

		private int findFreePort()
		{
			TcpListener l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();
			int port = ((IPEndPoint)l.LocalEndpoint).Port;
			l.Stop();
			return port;
		}
	}


}
