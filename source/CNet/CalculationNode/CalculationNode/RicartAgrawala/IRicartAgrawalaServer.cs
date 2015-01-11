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

		[XmlRpcMethod("Server.start")]
        bool Start(int seed);

		//[XmlRpcMethod( "Server.sum")]
		//bool Sum(int param);

		//[XmlRpcMethod( "Server.substract")]
		//bool Substract(int param);

		//[XmlRpcMethod( "Server.divide")]
		//bool Divide(int param);

		//[XmlRpcMethod( "Server.multiply")]
		//bool Multiply(int param);
    }
}
