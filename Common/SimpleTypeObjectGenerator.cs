using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	internal class SimpleTypeObjectGenerator
	{
		private long _index;
		private static readonly Dictionary<Type, Func<long, object>> DefaultGenerators = InitializeGenerators();

		private static Dictionary<Type, Func<long, object>> InitializeGenerators()
		{
			return new Dictionary<Type, Func<long, object>>
                {
                    { typeof(Boolean), index => true },
                    { typeof(Byte), index => (Byte)64 },
                    { typeof(Char), index => (Char)65 },
                    { typeof(DateTime), index => DateTime.Now },
                    { typeof(DateTimeOffset), index => new DateTimeOffset(DateTime.Now) },
                    { typeof(DBNull), index => DBNull.Value },
                    { typeof(Decimal), index => (Decimal)index },
                    { typeof(Double), index => (Double)(index + 0.1) },
                    { typeof(Guid), index => Guid.NewGuid() },
                    { typeof(Int16), index => (Int16)(index % Int16.MaxValue) },
                    { typeof(Int32), index => (Int32)(index % Int32.MaxValue) },
                    { typeof(Int64), index => (Int64)index },
                    { typeof(Object), index => new object() },
                    { typeof(SByte), index => (SByte)64 },
                    { typeof(Single), index => (Single)(index + 0.1) },
                    { 
                        typeof(String), index =>
                        {
                            return String.Format(CultureInfo.CurrentCulture, "sample string {0}", index);
                        }
                    },
                    { 
                        typeof(TimeSpan), index =>
                        {
                            return TimeSpan.FromTicks(1234567);
                        }
                    },
                    { typeof(UInt16), index => (UInt16)(index % UInt16.MaxValue) },
                    { typeof(UInt32), index => (UInt32)(index % UInt32.MaxValue) },
                    { typeof(UInt64), index => (UInt64)index },
                    { 
                        typeof(Uri), index =>
                        {
                            return new Uri(String.Format(CultureInfo.CurrentCulture, "http://webapihelppage{0}.com", index));
                        }
                    },
                };
		}

		public static bool CanGenerateObject(Type type)
		{
			if (type.IsEnum) return true;
			return DefaultGenerators.ContainsKey(type);
		}

		public object GenerateObject(Type type)
		{
			return DefaultGenerators[type](++_index);
		}
	}
}
