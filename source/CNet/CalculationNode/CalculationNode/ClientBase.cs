using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using CalculationNode.Extentions;

namespace CalculationNode
{
	public abstract class ClientBase : IClient, IDisposable
	{
		protected Uri BaseServerUri;
		protected ServiceHost HostObject;
		protected string LocalServerAddress;
		private bool Running { get; set; }

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
			ConsoleExtentions.Log(String.Format("Got response with {0} items from {1}",
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
				siblingProxy.SingOff(LocalServerAddress);
			}

			Parallel.ForEach(PeersData.GetAll(),
				peer =>
				{
					var siblingProxy = PeersData.GetChannel(peer);
					siblingProxy.SingOff(LocalServerAddress);
				});
			PeersData.Empty();
		}

		public void Start(int seed)
		{
			if (Running)
			{
				Console.WriteLine("Start rejected.");
				return;
			}

			//ConsoleExtentions.Log("\n\rStart command performed. Seed: " + seed);
			Parallel.ForEach(PeersData.GetAll(),
				peer =>
				{
					var se = (new Random().Next(10, 20));
					Console.WriteLine("Send: " + se);
					var siblingProxy = PeersData.GetChannel(peer);
					siblingProxy.Start(se);
				});
		}

		internal void StartSelf(int seed)
		{
			if (Running)
			{
				Console.WriteLine("Start rejected.");
				return;
			}

			EventGenerator.Start(this, 20000, 1000);
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
