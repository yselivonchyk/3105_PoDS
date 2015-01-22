using System;
using CookComputing.XmlRpc;

namespace CalculationNode
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRicartAgrawalaServer
    {
        [XmlRpcMethod( "Node.join")]
        Object[] Join (string address);

		[XmlRpcMethod("Node.signOff")]
		bool SignOff(string address);

		[XmlRpcMethod("Node.start")]
        bool Start(int seed);

		[XmlRpcMethod("Node.doCalculation")]
		bool DoCalculation(String operation, int value);

		[XmlRpcMethod("Node.receiveRequest")]
		bool RecieveAccess(int time, int id);

		[XmlRpcMethod("Node.finalizeSession")]
		void CalculationStopped();
    }

	/// <summary>
	/// RPC library extention for server interface
	/// </summary>
	public interface IRicardAgrawalaProxy : IXmlRpcProxy, IRicartAgrawalaServer
	{
	}
}
