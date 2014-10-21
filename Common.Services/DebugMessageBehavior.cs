using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common.Services
{
	public class DebugMessageInspector : IClientMessageInspector, IDispatchMessageInspector
	{
		#region client
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue);
			request = buffer.CreateMessage();
			Message m = buffer.CreateMessage();
			m.LogMessage("Client: BeforeSendRequest");
			return request;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
			reply = buffer.CreateMessage();
			Message m = buffer.CreateMessage();
			m.LogMessage("Client: AfterReceiveReply");
		}
		#endregion

		#region dispatcher
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue);
			request = buffer.CreateMessage();
			Message m = buffer.CreateMessage();
			m.LogMessage("Dispatcher: AfterReceiveRequest");
			return request;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
			reply = buffer.CreateMessage();
			Message m = buffer.CreateMessage();
			m.LogMessage("Dispatcher: BeforeSendReply");
		}
		#endregion
	}

	public static class MessageLogger
	{
		public static void LogMessage(this Message message, string messageType)
		{
			MemoryStream ms = new MemoryStream();
			XmlWriter writer = XmlWriter.Create(ms);
			message.WriteMessage(writer);
			writer.Flush();
			ms.Position = 0;
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.PreserveWhitespace = true;
			xmlDoc.Load(ms);

			Console.WriteLine("{0}: {1}", DateTime.Now, messageType);
			Logger.GetLogger().Info(messageType);
			Logger.GetLogger().Info(xmlDoc.OuterXml);
		}
	}

	public class DebugMessageBehavior : IEndpointBehavior
	{
		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new DebugMessageInspector());
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new DebugMessageInspector());
		}
	}

	public class DebugMessageBehaviorElement : BehaviorExtensionElement
	{
		protected override object CreateBehavior()
		{
			return new DebugMessageBehavior();
		}

		public override Type BehaviorType
		{
			get
			{
				return typeof(DebugMessageBehavior);
			}
		}
	}
}
