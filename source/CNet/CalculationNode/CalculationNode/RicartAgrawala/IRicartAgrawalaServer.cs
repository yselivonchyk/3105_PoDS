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

//		[XmlRpcMethod("Node.doTestCalc")]
//		bool DoCalculation(String operation, int value, int final);

		[XmlRpcMethod("Node.receiveRequest")]
		bool RecieveAccess(int time, int id);

		[XmlRpcMethod("Node.finalizeSession")]
		void CalculationStopped();

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
