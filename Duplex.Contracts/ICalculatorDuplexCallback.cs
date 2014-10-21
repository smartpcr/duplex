using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Duplex.Contracts
{
	public interface ICalculatorDuplexCallback
	{
		[OperationContract(IsOneWay = true)]
		void Equals(double result);
		[OperationContract(IsOneWay = true)]
		void Equation(string eqn);
	}
}
