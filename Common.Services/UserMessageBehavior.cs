using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
	public class UserMessageInspector : IClientMessageInspector
	{
		public const string Header_Name = "User";
		public const string Header_Namespace = "AAMVA.AVATTAR";

		#region client
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var userHeader = MessageHeader.CreateHeader(Header_Name, Header_Namespace, Environment.UserName);
			request.Headers.Add(userHeader);
			//object requestMessageObject;
			//HttpRequestMessageProperty prop;
			//request.Headers.Add(new MessageHeader());
			//if(request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out requestMessageObject))
			//{
			//	prop = requestMessageObject as HttpRequestMessageProperty;
			//	if(prop!=null && string.IsNullOrEmpty(prop.Headers[Header_Name]))
			//	{
			//		prop.Headers[Header_Name] = Environment.UserName;
			//	}
			//}
			//else
			//{
			//	prop = new HttpRequestMessageProperty();
			//	prop.Headers.Add(Header_Name, Environment.UserName);
			//	request.Properties.Add(HttpRequestMessageProperty.Name, prop);
			//}
			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}
		#endregion
	}

	public class UserMessageBehavior : IEndpointBehavior
	{
		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new UserMessageInspector());
		}
	}

	public class UserMessageBehaviorElement : BehaviorExtensionElement
	{
		protected override object CreateBehavior()
		{
			return new UserMessageBehavior();
		}

		public override Type BehaviorType
		{
			get
			{
				return typeof(UserMessageBehavior);
			}
		}
	}
}
