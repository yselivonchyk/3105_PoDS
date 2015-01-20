using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CalculationNode.Extentions;
using CalculationNode.RicartAgrawala;

namespace CalculationNode
{
	public abstract class ClientBase : IClient, IDisposable
	{
		public const int SesstionLength = 2000;
		public const int EventDelayAvg = 50;

		protected Uri BaseServerUri;
		protected ServiceHost HostObject;
		internal string LocalServerAddress;

		private static bool _running;

		public static bool Running
		{
			get
			{
				return _running;
			}
			private set
			{
				_running = value;
				RicardAgrawalaData.Calculations = 0;
			}
		}

		protected ClientBase(Uri baseServerUri)
		{
			BaseServerUri = baseServerUri;
			PeersData.LocalClient = this;
		}

		public void Join(Uri knownNodeUri)
		{
			// Add self 
			if (!LocalServerAddress.Equals(knownNodeUri.ToString()))
				JoinSingle(new Uri(LocalServerAddress));
			// save information about known node
			var peers = JoinSingle(knownNodeUri).Select(NetworkExtentions.TryBuildServerUri);
			PeersData.ID = peers.Count();
			Console.WriteLine("Local ID: {0}", PeersData.ID);
			var undiscoveredNodesAddresses = peers
				.Where(x => knownNodeUri != x && LocalServerAddress != x.ToString());
			// Make parallel non-blocking call to all other nodes
			Parallel.ForEach(
				undiscoveredNodesAddresses,
				siblingAddress => JoinSingle(siblingAddress));
		}

		// Get a connetion and call remote Join operation on a single node
		private string[] JoinSingle(Uri nodeAddress)
		{
			PeersData.Add(nodeAddress.ToString());
			var siblingProxy = PeersData.GetChannel(nodeAddress.ToString());

			var joinResponse = siblingProxy.Join(LocalServerAddress);
			ConsoleExtentions.Log(
				String.Format("Got response with {0} items from {1}",
				joinResponse.Length,
				nodeAddress));
			return joinResponse.Select(x => x.ToString()).ToArray();
		}

		public void SingOff()
		{
			ConsoleExtentions.Log("Empty request recieved.");

			foreach (var peer in PeersData.GetAll())
			{
				var siblingProxy = PeersData.GetChannel(peer);
				siblingProxy.SignOff(LocalServerAddress);
			}

			Parallel.ForEach(PeersData.GetAll(),
				peer => PeersData.GetChannel(peer).SignOff(LocalServerAddress));
			PeersData.Empty();
		}

		public void Start(int seed)
		{
			if (Running)
				return;
			Parallel.ForEach(PeersData.GetAll(),
				peer =>
				{
					Console.WriteLine("Send init request send: {0} - {1} ", seed, peer);
					var siblingProxy = PeersData.GetChannel(peer);
					siblingProxy.Initialize(seed);
					Console.WriteLine("Finished init request: {0} - {1} ", seed, peer);
				});
			Parallel.ForEach(PeersData.GetAll(),
				peer =>
				{
					Console.WriteLine("Send start request send: {0} - {1} ", seed, peer);
					var siblingProxy = PeersData.GetChannel(peer);
					siblingProxy.Start();
					Console.WriteLine("Finished start request: {0} - {1} ", seed, peer);
				});
			Console.WriteLine("\r\n\r\n");
		}

		internal void StartSelf(int seed)
		{
			if (Running)
				return;

			Running = true;
			EventGenerator.Start(this, SesstionLength, EventDelayAvg);
			// Wait for the late requests from peers.

			while (RicardAgrawalaData.GetQueueCount() != 0 
				|| RicardAgrawalaData.TimeFromRequest() < 2000)
			{
				Console.WriteLine("Actively wait.");
				Thread.Sleep(500);
			}

			ConsoleExtentions.Log(String.Format("Final value after {1} calculations: {0}",
			RicardAgrawalaData.CurrentValue,
			RicardAgrawalaData.Calculations));
				
			Running = false;
		}

		public abstract void Sum(int param);

		public abstract void Subtract(int param);

		public abstract void Divide(int param);

		public abstract void Multiply(int param);
		public void Dispose()
		{
			HostObject.Close();
		}
	}
}
