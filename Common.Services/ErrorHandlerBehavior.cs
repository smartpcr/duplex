using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
	[DataContract(Namespace = "http://aamva.org/avattar/errors")]
	public class MyCustomFault : FaultException
	{
		public string AssociationId { get; set; }
		public string User { get; set; }
		public string Error { get; set; }

		public MyCustomFault(string reason)
			: base(reason)
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ErrorHandlerBehaviorAttribute : Attribute, IServiceBehavior, IErrorHandler
	{
		public ErrorHandlerBehaviorAttribute()
		{
			ServiceType = null;
		}

		/// <summary>
		/// 
		/// </summary>
		protected Type ServiceType { get; set; }

		#region IServiceBehavior Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			ServiceType = serviceDescription.ServiceType;

			foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
			{
				dispatcher.ErrorHandlers.Add(this);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			//throw new Exception("The method or operation is not implemented.");
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceDescription"></param>
		/// <param name="serviceHostBase"></param>
		/// <param name="endpoints"></param>
		/// <param name="bindingParameters"></param>
		void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IErrorHandler Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		bool IErrorHandler.HandleError(Exception error)
		{
			if (error != null)
			{
				string id = string.Empty;
				string user = "Unknown";
				if (error.Data["ErrorKey"] != null)
				{
					id = (string)error.Data["ErrorKey"];
				}
				if (error.Data["User"] != null)
				{
					user = (string)error.Data["User"];
				}
				string errorMessage = string.Format("ID={0}\n\tUser={1}\n\t{2}", id, user, error.Message);
				Logger.GetLogger().Error(errorMessage, error);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="error"></param>
		/// <param name="version"></param>
		/// <param name="fault"></param>
		void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			string associationId = Guid.NewGuid().ToString();
			error.Data["ErrorKey"] = associationId;
			string user = "Unknown";
			OperationContext context = OperationContext.Current;
			if (context != null)
			{
				user = context.IncomingMessageHeaders.GetHeader<string>(
					UserMessageInspector.Header_Name,
					UserMessageInspector.Header_Namespace);
			}
			error.Data["User"] = user ?? "Unknown";

			string reason = string.Format("{2} \nAssociation={0} \nUser={1}", associationId, user, error.Message);

			MyCustomFault customFault = new MyCustomFault(reason)
			{
				AssociationId = associationId,
				User = user,
				Error = error.Message,
			};
			FaultException<MyCustomFault> faultEx = new FaultException<MyCustomFault>(
				customFault, error.Message,
				FaultCode.CreateSenderFaultCode("MyCustomFault", "http://aamva.org/avattar/errors"));
			MessageFault mf = customFault.CreateMessageFault();
			fault = Message.CreateMessage(version, mf, faultEx.Action);
		}

		#endregion
	}

	public class ErrorBehaviorExtensionElement : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof(ErrorHandlerBehaviorAttribute); }
		}

		protected override object CreateBehavior()
		{
			return new ErrorHandlerBehaviorAttribute();
		}
	}
}
