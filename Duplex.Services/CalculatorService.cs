﻿using Common.Services;
using Duplex.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Duplex.Services
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
	public class CalculatorService : ServiceImplBase, ICalculatorDuplex
	{
		double result = 0.0D;
		string equation;

		public CalculatorService()
		{
			equation = result.ToString();
		}

		public void Clear()
		{
			Callback.Equation(equation + " = " + result.ToString());
			equation = result.ToString();
		}

		public void AddTo(double n)
		{
			result += n;
			equation += " + " + n.ToString();
			Callback.Equals(result);
		}

		public void SubtractFrom(double n)
		{
			result -= n;
			equation += " - " + n.ToString();
			Callback.Equals(result);
		}

		public void MultiplyBy(double n)
		{
			result *= n;
			equation += " * " + n.ToString();
			Callback.Equals(result);
		}

		public void DivideBy(double n)
		{
			result /= n;
			equation += " / " + n.ToString();
			Callback.Equals(result);
		}

		ICalculatorDuplexCallback Callback
		{
			get
			{
				return OperationContext.Current.GetCallbackChannel<ICalculatorDuplexCallback>();
			}
		}
	}
}
