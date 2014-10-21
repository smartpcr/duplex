using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
	public abstract class ServiceImplBase
	{
		protected string User { get; private set; }

		protected ServiceImplBase()
		{
			OperationContext context = OperationContext.Current;
			if (context != null)
			{
				this.User = context.IncomingMessageHeaders.GetHeader<string>(
					UserMessageInspector.Header_Name, 
					UserMessageInspector.Header_Namespace);
			}
		}

		protected void HandleError(Exception ex)
		{
			Guid id = Guid.NewGuid();
			string errorMessage = string.Format("Id: {0}, User: {1}, Error: {2}", id, this.User, ex.Message);
			Logger.GetLogger().Error(errorMessage, ex);
			throw new FaultException(ex.Message);
		}
	}
}
