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

		[XmlRpcMethod( "Node.singOff")]
		bool SingOff(string address);

		[XmlRpcMethod("Node.start")]
        bool Start(int seed);

		[XmlRpcMethod("Node.doCalculation")]
		bool DoCalculation(String operation, int value);

		//[XmlRpcMethod( "Server.sum")]
		//bool Sum(int param);

		//[XmlRpcMethod( "Server.substract")]
		//bool Subtract(int param);

		//[XmlRpcMethod( "Server.divide")]
		//bool Divide(int param);

		//[XmlRpcMethod( "Server.multiply")]
		//bool Multiply(int param);
    }

	/// <summary>
	/// RPC library extention for server interface
	/// </summary>
	public interface IRicardAgrawalaProxy : IXmlRpcProxy, IRicartAgrawalaServer
	{
	}
}
