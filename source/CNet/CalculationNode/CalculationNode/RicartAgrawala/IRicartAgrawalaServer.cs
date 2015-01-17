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

		[XmlRpcMethod("Node.setInitValue")]
        bool Init(int seed);

		[XmlRpcMethod("Node.start")]
		bool Start();

		[XmlRpcMethod("Node.doCalculation")]
		bool DoCalculation(String operation, int value);

//		[XmlRpcMethod("Node.doTestCalc")]
//		bool DoCalculation(String operation, int value, int final);

		[XmlRpcMethod("Node.receiveRequest")]
		bool RecieveAccess(String address, int time, int id);

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
