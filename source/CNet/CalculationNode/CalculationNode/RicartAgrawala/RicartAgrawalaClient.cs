using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CalculationNode.Extentions;
using CookComputing.XmlRpc;

namespace CalculationNode.RicartAgrawala
{
	/// <summary>
	/// Implements all algorithms required by RicardAgrawala routin
	/// </summary>
	public class RicartAgrawalaClient : ClientBase
	{
		public RicartAgrawalaClient(Uri baseServerUri) : base(baseServerUri)
		{
			var localServerUri = BaseServerUri;
			LocalServerAddress = localServerUri.ToString();

			var props = new Hashtable();
			props["name"] = "Server";
			props["port"] = baseServerUri.Port;
			var channel = new HttpChannel(
			  props,
			  null,
			  new XmlRpcServerFormatterSinkProvider()
		   );
			ChannelServices.RegisterChannel(channel, false);
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof (RicartAgrawalaServer),
				"xmlrpc",
				WellKnownObjectMode.Singleton);
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
