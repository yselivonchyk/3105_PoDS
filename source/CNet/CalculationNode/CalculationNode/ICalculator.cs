using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CalculationNode
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(Namespace = "https://github.com/yselivonchyk/3105_PoDS")]
    public interface ICalculator
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
    }
}
