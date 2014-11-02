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
        string SingOff(string address);

        [OperationContract(Action = "start")]
        void Start(int seed);

        [OperationContract(Action = "sum")]
        void Sum(int param);

        [OperationContract(Action = "substract")]
        void Substract(int param);

        [OperationContract(Action = "divide")]
        void Divide(int param);

        [OperationContract(Action = "multiply")]
        void Multiply(int param);

		[OperationContract(Action = "echo")]
		string Echo(string param);
    }
}
