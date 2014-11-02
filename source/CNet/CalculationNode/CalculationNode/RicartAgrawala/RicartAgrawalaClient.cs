using System;
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
