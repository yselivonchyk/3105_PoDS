using System.ServiceModel;

namespace CalculationNode
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(Namespace = "https://github.com/yselivonchyk/3105_PoDS")]
    public interface IRicartAgrawalaServer
    {
        [OperationContract(Action = "Server.join")]
        string[] Join (string address);

        [OperationContract(Action = "Server.singoff")]
		bool SingOff(string address);

        [OperationContract(Action = "Server.start")]
        bool Start(int seed);

        [OperationContract(Action = "Server.sum")]
		bool Sum(int param);

        [OperationContract(Action = "Server.substract")]
		bool Substract(int param);

        [OperationContract(Action = "Server.divide")]
		bool Divide(int param);

        [OperationContract(Action = "Server.multiply")]
		bool Multiply(int param);
    }
}
