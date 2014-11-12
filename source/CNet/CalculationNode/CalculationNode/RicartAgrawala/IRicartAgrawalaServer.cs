using System.ServiceModel;

namespace CalculationNode
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(Namespace = "https://github.com/yselivonchyk/3105_PoDS")]
    public interface IRicartAgrawalaServer
    {
        [OperationContract(Action = "join")]
        string[] Join (string address);

        [OperationContract(Action = "singoff")]
		bool SingOff(string address);

        [OperationContract(Action = "start")]
        bool Start(int seed);

        [OperationContract(Action = "sum")]
		bool Sum(int param);

        [OperationContract(Action = "substract")]
		bool Substract(int param);

        [OperationContract(Action = "divide")]
		bool Divide(int param);

        [OperationContract(Action = "multiply")]
		bool Multiply(int param);
    }
}
