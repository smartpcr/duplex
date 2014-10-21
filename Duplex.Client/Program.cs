using Duplex.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Duplex.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			// Construct InstanceContext to handle messages on callback interface
			InstanceContext instanceContext = new InstanceContext(new CallbackHandler());

			// Create a client
			using(DuplexChannelFactory<ICalculatorDuplex> cf = 
				new DuplexChannelFactory<ICalculatorDuplex>(instanceContext))
			{
				EndpointAddress endPoint = new EndpointAddress(new Uri("http://localhost:54321/Calcularor"));
				ICalculatorDuplex client = cf.CreateChannel(endPoint);
				client.AddTo(120);
			}
		}
	}
}
