using System;

namespace CalculationNode
{
	/// <summary>
	/// Client contract that will be used by event generator
	/// </summary>
	public interface IClient
	{
		void Join(Uri knownNodeUri);

		void SingOff();

		void Start(int seed);

		void Sum(int param);

		void Subtract(int param);

		void Divide(int param);

		void Multiply(int param);
	}
}
