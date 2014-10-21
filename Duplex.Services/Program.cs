using Duplex.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Duplex.Services
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceHost svcHost = new ServiceHost(typeof(CalculatorService), new Uri("http://localhost:54321/Calcularor"));
			svcHost.Open();
		}
	}
}
