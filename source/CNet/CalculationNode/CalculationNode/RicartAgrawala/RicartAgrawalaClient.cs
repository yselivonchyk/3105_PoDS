using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Samples.XmlRpc;

namespace CalculationNode.RicartAgrawala
{
	/// <summary>
	/// Implements all algorithms required by RicardAgrawala routin
	/// </summary>
	public class RicartAgrawalaClient : ClientBase
	{
		public RicartAgrawalaClient(Uri baseServerUri) : base(baseServerUri)
		{
			// Start local server to process request from other nodes
			var localServerUri = BaseServerUri;
			LocalServerAddress = localServerUri.ToString();
			HostObject = new ServiceHost(typeof(RicartAgrawalaServer));
			var epXmlRpc = HostObject.AddServiceEndpoint(typeof(IRicartAgrawalaServer),
				new WebHttpBinding(WebHttpSecurityMode.None), localServerUri);
			epXmlRpc.Behaviors.Add(new XmlRpcEndpointBehavior());
			HostObject.Open();
			Console.WriteLine("Ricard-Agrawala node listning at {0}", epXmlRpc.ListenUri);
		}

		public override void Join(Uri knownNodeUri)
		{
			// save information about known node
			var knownNodeAddress = knownNodeUri.ToString();
			PeersData.Add(knownNodeAddress);
			// send join request to known node
			var nodeProxy = PeersData.GetChannel(knownNodeAddress);
			var allNodesAddresses = nodeProxy.Join(LocalServerAddress);
			var undiscoveredNodesAddresses = allNodesAddresses.Where(x => !x.Equals(knownNodeAddress));
			
			foreach (var siblingAddress in undiscoveredNodesAddresses)
			{
				PeersData.Add(siblingAddress);
				var siblingProxy = PeersData.GetChannel(siblingAddress);
				var joinResponse = siblingProxy.Join(LocalServerAddress);
				ConsoleExtentions.Log(String.Format("Got response with {0} items", joinResponse.Length));
				//response.ToList().ForEach(x => Console.WriteLine("{0} - {1}", fellow, x));
			}
		}

		public override void Start(int seed)
		{
			throw new NotImplementedException();
		}

		public override void Sum(int param)
		{
			throw new NotImplementedException();
		}

		public override void Substract(int param)
		{
			throw new NotImplementedException();
		}

		public override void Divide(int param)
		{
			throw new NotImplementedException();
		}

		public override void Multiply(int param)
		{
			throw new NotImplementedException();
		}
	}
}
