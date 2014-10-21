using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CalculationNode
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public abstract class CalculatorBase
    {
        public int CurrentValue { get; protected set; }

        public string[] Join(string address)
        {
            throw new NotImplementedException();
        }

        public void SingOff(string address)
        {
            throw new NotImplementedException();
        }

        public void Start(int seed)
        {
            CurrentValue = seed;
        }

        public abstract void Sum(int param);
        public abstract void Substract(int param);
        public abstract void Divide(int param);
        public abstract void Multiply(int param);
    }
}
