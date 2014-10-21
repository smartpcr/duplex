using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Common
{
	public static class Logger
	{
		//private static IKernel _appKernel;

		static Logger()
		{
			log4net.Config.XmlConfigurator.Configure();
			//var settings = new NinjectSettings { LoadExtensions = false };
			//_appKernel = new StandardKernel(settings, new INinjectModule[] { new Log4NetModule() });
			//_appKernel.Bind<ILogger>().To<Log4NetWrapper>();
		}

		public static ILog GetLogger()
		{
			var stack = new StackTrace();
			var frame = stack.GetFrame(1);
			return LogManager.GetLogger(frame.GetMethod().DeclaringType);
		}
	}
}
