using System;

namespace CalculationNode
{
	/// <summary>
	/// Client contract that will be used by event generator
	/// </summary>
	public interface IClient
	{
		void Join(Uri fellowAddress);

		void SingOff(string address);

		void Start(int seed);

		void Sum(int param);

		void Substract(int param);

		void Divide(int param);

		void Multiply(int param);
	}
}
