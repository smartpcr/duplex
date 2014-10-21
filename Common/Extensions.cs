using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Common
{
	public static class Extensions
	{
		#region xml

		public static string XEncode(this string input)
		{
			if (input == null) return string.Empty;
			return XmlConvert.EncodeName(input);
		}

		public static string XDecode(this string input)
		{
			if (input == null) return string.Empty;
			return XmlConvert.DecodeName(input);
		}

		public static int XIntVal(this XAttribute xAttr)
		{
			if (xAttr != null && !string.IsNullOrEmpty(xAttr.Value))
				return xAttr.Value.IntValue();
			return 0;
		}

		public static int XIntVal(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return xe.Attribute(attrName).XIntVal();
			return 0;
		}

		public static double? XDouble(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return double.Parse(xe.Attribute(attrName).Value);
			return default(double?);
		}

		public static double XDoubleValue(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return double.Parse(xe.Attribute(attrName).Value);
			return 0;
		}

		public static int? XInt(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return Convert.ToInt32(xe.Attribute(attrName).Value);
			return default(int?);
		}

		public static int XIntValue(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return Convert.ToInt32(xe.Attribute(attrName).Value);
			return 0;
		}

		public static decimal? XDecimal(this XElement xe, string attrName)
		{
			if (xe == null) return default(decimal?);
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return default(decimal?);
			return Convert.ToDecimal(attr.Value);
		}

		public static decimal XDecimalValue(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return Convert.ToDecimal(xe.Attribute(attrName).Value);
			return 0;
		}

		public static string XStr(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return xe.Attribute(attrName).Value;
			return null;
		}

		public static string XStrValue(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return xe.Attribute(attrName).Value;
			return string.Empty;
		}

		public static bool? XBool(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return xe.Attribute(attrName).Value.BoolValue();
			return default(bool?);
		}

		public static bool XBoolValue(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return xe.Attribute(attrName).Value.BoolValue();
			return false;
		}

		public static DateTime? XDateTime(this XElement xe, string attrName)
		{
			if (xe == null) return null;
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return null;
			return Convert.ToDateTime(attr.Value);
		}

		public static DateTime XDateTimeValue(this XElement xe, string attrName)
		{
			if (xe == null) return new DateTime(1900, 1, 1);
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return new DateTime(1900, 1, 1);
			return Convert.ToDateTime(attr.Value);
		}

		public static TimeSpan? XTimeSpan(this XElement xe, string attrName)
		{
			if (xe == null) return null;
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return null;
			return TimeSpan.Parse(attr.Value);
		}

		public static T XEnum<T>(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
				return (T)Enum.Parse(typeof(T), xe.Attribute(attrName).Value, true);
			return default(T);
		}

		public static UInt32 XUIntVal(this XElement xe, string attrName)
		{
			if (xe == null) return default(UInt32);
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return default(UInt32);
			return Convert.ToUInt32(attr.Value);
		}

		public static ulong? XULong(this XElement xe, string attrName)
		{
			if (xe == null) return default(ulong?);
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return default(ulong?);
			return Convert.ToUInt64(attr.Value);
		}

		public static ulong XULongValue(this XElement xe, string attrName)
		{
			if (xe == null) return 0;
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return 0;
			return Convert.ToUInt64(attr.Value);
		}

		public static Guid XGuid(this XElement xe, string attrName)
		{
			if (xe == null) return Guid.Empty;
			XAttribute attr = xe.Attribute(attrName);
			if (attr == null) return Guid.Empty;
			return new Guid(attr.Value);
		}

		public static int[] XIntArray(this XElement xe, string attrName)
		{
			if (xe != null && xe.Attribute(attrName) != null)
			{
				string attrValue = xe.Attribute(attrName).Value;
				List<int> intList = new List<int>();
				string[] items = attrValue.Split(new[] { ',' });
				foreach (string item in items)
				{
					if (!string.IsNullOrEmpty(item.Trim()))
					{
						int intValue = int.Parse(item.Trim());
						if (!intList.Contains(intValue)) intList.Add(intValue);
					}
				}
				return intList.ToArray();
			}
			return null;
		}

		public static void AddAttribute(this XElement xe, string attrName, object value)
		{
			if (xe == null) return;
			if (!string.IsNullOrEmpty(attrName) && value != null && value != DBNull.Value)
			{
				XAttribute attr = xe.Attribute(attrName);
				if (attr == null)
					xe.Add(new XAttribute(attrName, value.ToString()));
				else
					attr.Value = value.ToString();
			}
		}

		public static XElement DbXml(this object value)
		{
			if (value == null || value == DBNull.Value) return null;
			string xml = value.ToString();
			return XElement.Parse(xml, LoadOptions.PreserveWhitespace);
		}
		#endregion

		#region value
		public static string DbString(this object value)
		{
			return (value == null || value == DBNull.Value) ? null : Convert.ToString(value);
		}

		public static string StringValue(this object value)
		{
			return (value == null || value == DBNull.Value) ? string.Empty : Convert.ToString(value);
		}

		public static bool? DbBool(this object value)
		{
			return (value != null && value != DBNull.Value) ? Convert.ToBoolean(value) : default(bool?);
		}

		public static bool BoolValue(this object value)
		{
			return (value != null && value != DBNull.Value) && Convert.ToBoolean(value);
		}

		public static int? DbInt(this object value)
		{
			return ((value == null || value == DBNull.Value) ? default(int?) : Convert.ToInt32(value, CultureInfo.InvariantCulture));
		}

		public static int IntValue(this object value)
		{
			if (value == null || value == DBNull.Value) return 0;
			string input = value.ToString().Trim();
			if (!string.IsNullOrEmpty(input))
				return input.ConvertToInt();
			return 0;
		}
		static Regex floatingNumberRegex = new Regex(@"[-+]?[0-9]*\.[0-9]*", RegexOptions.Compiled);

		public static int ConvertToInt(this string input)
		{
			if (floatingNumberRegex.IsMatch(input))
			{
				double doubleValue = double.Parse(input);
				return (int)doubleValue;
			}
			return int.Parse(input);
		}

		public static UInt32 UIntValue(this object value)
		{
			return ((value == null || value == DBNull.Value) ? 0 : Convert.ToUInt32(value, CultureInfo.InvariantCulture));
		}

		public static Int16? DbInt16(this object value)
		{
			return ((value == null || value == DBNull.Value) ? default(Int16?) : Convert.ToInt16(value, CultureInfo.InvariantCulture));
		}

		public static Int16 Int16Value(this object value)
		{
			return ((value == null || value == DBNull.Value) ? default(Int16) : Convert.ToInt16(value, CultureInfo.InvariantCulture));
		}

		public static Int64 Int64Value(this object value)
		{
			if (value == null || value == DBNull.Value) return 0;
			return Convert.ToInt64(value);
		}

		public static Int64? DbInt64(this object value)
		{
			if (value == null || value == DBNull.Value) return default(Int64?);
			return Convert.ToInt64(value);
		}

		public static ulong ULongValue(this object value)
		{
			if (value == null || value == DBNull.Value) return 0;
			return Convert.ToUInt64(value);
		}

		public static ulong? DbULong(this object value)
		{
			if (value == null || value == DBNull.Value) return default(ulong);
			return Convert.ToUInt64(value);
		}

		public static decimal? DbDecimal(this object value)
		{
			return ((value == null || value == DBNull.Value) ? default(decimal?) : Convert.ToDecimal(value, CultureInfo.InvariantCulture));
		}

		public static decimal DecimalValue(this object value)
		{
			return ((value == null || value == DBNull.Value) ? 0 : Convert.ToDecimal(value, CultureInfo.InvariantCulture));
		}

		public static double? DbDouble(this object value)
		{
			return ((value == null || value == DBNull.Value) ? default(double?) : Convert.ToDouble(value, CultureInfo.InvariantCulture));
		}

		public static double DoubleValue(this object value)
		{
			return (value == null || value == DBNull.Value) ? 0 : Convert.ToDouble(value, CultureInfo.InvariantCulture);
		}

		public static float FloatValue(this object value)
		{
			if (value == null || value == DBNull.Value) return 0F;
			if (value is string)
			{
				string strValue = value.ToString().Trim();
				if (!string.IsNullOrEmpty(strValue))
					return float.Parse(strValue);
				return 0F;
			}
			if (value is double || value is decimal)
			{
				return Convert.ToSingle(value);
			}
			return (float)value;
		}

		public static DateTime? DbDate(this object value)
		{
			return ((value == null || value == DBNull.Value)
						? default(DateTime?)
						: Convert.ToDateTime(value));
		}

		public static DateTime DateValue(this object value)
		{
			return ((value == null || value == DBNull.Value)
						? new DateTime(1900, 1, 1)
						: Convert.ToDateTime(value));
		}

		public static DateTime DateValue(this object value, DateTime defaultValue)
		{
			return ((value == null || value == DBNull.Value)
						? defaultValue
						: Convert.ToDateTime(value));
		}

		public static TimeSpan? DbSpan(this object value)
		{
			if (value == null || value == DBNull.Value) return default(TimeSpan?);
			if (value is TimeSpan) return (TimeSpan)value;
			return (TimeSpan)Convert.ChangeType(value, typeof(TimeSpan));
		}

		public static TimeSpan SpanValue(this object value)
		{
			if (value == null || value == DBNull.Value) return new TimeSpan(0, 0, 0);
			if (value is TimeSpan) return (TimeSpan)value;
			return (TimeSpan)Convert.ChangeType(value, typeof(TimeSpan));
		}

		public static T DbEnum<T>(this object value) where T : struct ,IComparable
		{
			if (value == null || value == DBNull.Value) return default(T);
			if (value is string)
			{
				if (string.IsNullOrEmpty((string)value)) return default(T);
				T enumVal;
				if (Enum.TryParse((string)value, true, out enumVal))
					return enumVal;
			}
			if (value is int)
				return (T)value;
			if (value is Enum)
			{
				if (value.GetType() == typeof(T))
					return (T)value;
				return (T)Enum.Parse(typeof(T), value.ToString(), true);
			}
			return default(T);
		}

		public static T EnumValue<T>(this object value) where T : struct ,IComparable
		{
			if (value == null || value == DBNull.Value) return default(T);
			if (value is string)
			{
				if (string.IsNullOrEmpty((string)value)) return default(T);
				return (T)Enum.Parse(typeof(T), (string)value, true);
			}
			if (value is int)
				return (T)value;
			if (value is Enum)
			{
				if (value.GetType() == typeof(T))
					return (T)value;
				return (T)Enum.Parse(typeof(T), value.ToString(), true);
			}
			return default(T);
		}

		public static Guid? DbGuid(this object value)
		{
			if (value == null || value == DBNull.Value) return default(Guid?);
			var strValue = value.ToString().Trim();
			if (strValue.Length == 0) return default(Guid?);
			return new Guid(value.ToString());
		}

		public static Guid GuidValue(this object value)
		{
			if (value == null || value == DBNull.Value) return default(Guid);
			var strValue = value.ToString().Trim();
			if (strValue.Length == 0) return default(Guid);
			return new Guid(value.ToString());
		}

		public static byte[] DbBytes(this object value)
		{
			if (value == null || value == DBNull.Value) return null;
			return value as byte[];
		}
		#endregion

		#region type
		public static Type SystemType(this object value)
		{
			if (value == null || value == DBNull.Value)
				return typeof(string);
			string typeName = value.DbString();
			if (!string.IsNullOrEmpty(typeName))
				return Type.GetType(typeName);
			return typeof(string);
		}

		public static bool IsPrimitiveType(this Type type)
		{
			var types = new[]
                          {
                              typeof (Enum),
                              typeof (String),
                              typeof (Char),
                              typeof (Boolean),
                              typeof (Byte),
                              typeof (Int16),
                              typeof (Int32),
                              typeof (Int64),
                              typeof (Single),
                              typeof (Double),
                              typeof (Decimal),
                              typeof (SByte),
                              typeof (UInt16),
                              typeof (UInt32),
                              typeof (UInt64),
                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan),
                          };
			return type.IsPrimitive || types.Contains(type) || type.IsEnum;
		}

		public static bool IsCompatibleWithType(this object value, Type type)
		{
			if (value == null)
				return false;
			if (type.IsPrimitiveType())
			{
				try
				{
					Convert.ChangeType(value, type);
					return true;
				}
				catch
				{
					return false;
				}
			}
			return value.GetType() == type;
		}

		public static bool? IsCompatibleWithType(this string value, string typeName)
		{
			Type type = Type.GetType(typeName);
			if (value == null || type == null)
				return null;
			if (type == typeof(string))
				return true;
			if (type.IsPrimitiveType())
			{
				try
				{
					Convert.ChangeType(value, type);
					return true;
				}
				catch
				{
					return false;
				}
			}
			return value.GetType() == type;
		}
		#endregion

		#region join
		public static string Join(this IEnumerable<string> source)
		{
			StringBuilder sb = new StringBuilder();
			if (source != null)
			{
				var enumerator = source.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (sb.Length > 0) sb.Append(",");
					sb.Append(enumerator.Current);
				}
			}
			return sb.ToString();
		}

		public static string Join(this IEnumerable<string> source, string delimiter)
		{
			StringBuilder sb = new StringBuilder();
			if (source != null)
			{
				var enumerator = source.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (sb.Length > 0) sb.Append(delimiter);
					sb.Append(enumerator.Current);
				}
			}
			return sb.ToString();
		}

		public static string Join(this IEnumerable<int> source)
		{
			StringBuilder sb = new StringBuilder();
			if (source != null)
			{
				var enumerator = source.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (sb.Length > 0) sb.Append(",");
					sb.Append(enumerator.Current);
				}
			}
			return sb.ToString();
		}
		#endregion

		#region split

		public static List<string> ToStringList(this string input, string splitBy = ",")
		{
			if (!string.IsNullOrEmpty(input))
			{
				return input.Split(new string[] { splitBy }, StringSplitOptions.RemoveEmptyEntries).ToList();
			}
			return new List<string>();
		}

		public static List<int> ToIntList(this string input, string splitBy = ",")
		{
			if (!string.IsNullOrEmpty(input))
			{
				var strList = input.Split(new string[] { splitBy }, StringSplitOptions.RemoveEmptyEntries).ToList();
				List<int> intList = new List<int>();
				foreach (string item in strList)
				{
					intList.Add(item.IntValue());
				}
				return intList;
			}
			return new List<int>();
		}
		#endregion

		#region hex
		public static string ToHexString(this byte[] value)
		{
			return ByteArrayToHexString(value);
		}

		public static string ByteArrayToHexString(byte[] pBytes)
		{
			string lString = pBytes.Aggregate("", (current, lB) => current + (lB.ToString("X2") + " "));
			return lString.Trim();
		}

		public static byte[] HexStringToByteArray(this string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				var bytes = Enumerable.Range(0, value.Length)
						 .Where(x => x % 2 == 0)
						 .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
						 .ToArray();
				return bytes;
			}
			return null;
		}

		public static byte[] ToHexBytes(this string hexString, out int discarded)
		{
			discarded = 0;
			string newString = "";
			// remove all none A-F, 0-9, characters
			for (int i = 0; i < hexString.Length; i++)
			{
				char c = hexString[i];
				if (IsHexDigit(c))
					newString += c;
				else
					discarded++;
			}
			// if odd number of characters, discard last character
			if (newString.Length % 2 != 0)
			{
				discarded++;
				newString = newString.Substring(0, newString.Length - 1);
			}

			int byteLength = newString.Length / 2;
			byte[] bytes = new byte[byteLength];
			string hex;
			int j = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				hex = new String(new Char[] { newString[j], newString[j + 1] });
				bytes[i] = HexToByte(hex);
				j = j + 2;
			}
			return bytes;
		}

		/// <summary>
		/// Determines if given string is in proper hexadecimal string format
		/// </summary>
		/// <param name="hexString"></param>
		/// <returns></returns>
		public static bool InHexFormat(string hexString)
		{
			bool hexFormat = true;

			foreach (char digit in hexString)
			{
				if (!IsHexDigit(digit))
				{
					hexFormat = false;
					break;
				}
			}
			return hexFormat;
		}

		/// <summary>
		/// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
		/// </summary>
		/// <param name="c">Character to test</param>
		/// <returns>true if hex digit, false if not</returns>
		private static bool IsHexDigit(Char c)
		{
			int numChar;
			int numA = Convert.ToInt32('A');
			int num1 = Convert.ToInt32('0');
			c = Char.ToUpper(c);
			numChar = Convert.ToInt32(c);
			if (numChar >= numA && numChar < (numA + 6))
				return true;
			if (numChar >= num1 && numChar < (num1 + 10))
				return true;
			return false;
		}

		/// <summary>
		/// Converts 1 or 2 character string into equivalant byte value
		/// </summary>
		/// <param name="hex">1 or 2 character string</param>
		/// <returns>byte</returns>
		private static byte HexToByte(string hex)
		{
			if (hex.Length > 2 || hex.Length <= 0)
				throw new ArgumentException("hex must be 1 or 2 characters in length");
			byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			return newByte;
		}
		#endregion

		#region dictionary
		public static Dictionary<string, object> ReadDictionary(this XElement xe)
		{
			Dictionary<string, object> vars = new Dictionary<string, object>();
			if (xe != null)
			{
				foreach (var varElem in xe.Elements("Variable"))
				{
					string name = varElem.XStr("Name");
					object value = null;
					string typeName = varElem.XStr("Type");
					if (!string.IsNullOrEmpty(typeName))
					{
						Type type = Type.GetType(typeName);
						if (type != null)
						{
							if (type.IsValueType)
							{
								string strValue = varElem.XStr("Value");
								if (!string.IsNullOrEmpty(strValue))
								{
									if (type == typeof(string))
										value = strValue;
									else
										value = Convert.ChangeType(strValue, type);
								}
							}
							else
							{
								XElement valueElem = xe.Element("Value");
								if (valueElem != null)
								{
									XmlSerializer serializer = new XmlSerializer(type);
									value = serializer.Deserialize(XmlReader.Create(new StringReader(valueElem.ToString())));
								}
							}
						}
					}

					vars.Add(name, value);
				}
			}
			return vars;
		}

		public static XElement ToXml(this Dictionary<string, object> variables)
		{
			XElement xe = new XElement("Variables");
			if (variables != null && variables.Count > 0)
			{
				foreach (string name in variables.Keys)
				{
					XElement varElem = new XElement("Variable", new XAttribute("Name", name));
					xe.Add(varElem);
					object value = variables[name];
					if (value != null)
					{
						Type type = value.GetType();
						string typeName = type.FullName;
						varElem.Add(new XAttribute("Type", typeName));
						if (type.IsValueType)
						{
							varElem.Add(new XAttribute("Value", value.ToString()));
						}
						else
						{
							XElement valueElem = new XElement("Value");
							XmlSerializer serializer = new XmlSerializer(type);
							var memStream = new MemoryStream();
							serializer.Serialize(memStream, value);
							byte[] buffer = new byte[memStream.Length];
							memStream.Write(buffer, 0, buffer.Length);
							string xml = Encoding.Default.GetString(buffer);
							valueElem.Add(XElement.Parse(xml), LoadOptions.PreserveWhitespace);
							varElem.Add(valueElem);
						}
					}
				}
			}
			return xe;
		}
		#endregion

		#region base64
		public static string ToBase64(this string input)
		{
			byte[] toEncodeAsBytes = Encoding.Default.GetBytes(input);
			string returnValue = Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		public static string FromBase64(this string input)
		{
			byte[] encodedDataAsBytes = Convert.FromBase64String(input);
			string returnValue = Encoding.Default.GetString(encodedDataAsBytes);
			return returnValue;
		}
		#endregion

		#region convert
		public static object CompatibleType(this object input, Type targetType)
		{
			if (input == null) return null;

			if (targetType.IsEnum)
			{
				if (input is string)
				{
					string inputStr = input.ToString();
					try
					{
						int enumValue;
						if (int.TryParse(inputStr, out enumValue))
							return Convert.ChangeType(enumValue, targetType);
						return Enum.Parse(targetType, inputStr);
					}
					catch
					{
						targetType.DefaultValue();
					}
				}
			}

			if (input is string && (targetType == typeof(bool) || targetType == typeof(Boolean)))
			{
				string strInput = (string)input;
				if (strInput == "Y" || strInput == "T" || strInput == "1") return true;
				if (strInput == "N" || strInput == "F" || strInput == "0") return false;
			}
			try
			{
				object valueReturned = ValueConverter.ChangeType(input, targetType);
				if (valueReturned.GetType() == targetType) return valueReturned;
				return null;
			}
			catch
			{
				return null;
			}
		}

		#endregion

		#region format
		public static string PadLeft(this int input, int length)
		{
			return input.ToString().PadLeft(length, '0');
		}
		#endregion

		#region serialization

		public static bool? IsSimpleValue(this object value)
		{
			if (value == null) return null;
			if (SimpleTypeObjectGenerator.CanGenerateObject(value.GetType()))
				return true;
			return false;
		}

		public static bool IsSimpleType(this Type type)
		{
			return SimpleTypeObjectGenerator.CanGenerateObject(type);
		}

		public const string ComplexObjectPrefix = "COMPLEX||";

		public static string Serialize(this object value)
		{
			if (value == null) return null;
			if (SimpleTypeObjectGenerator.CanGenerateObject(value.GetType()))
				return value.ToString();

			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, value);
			memoryStream.Flush();
			memoryStream.Position = 0;
			string output = Convert.ToBase64String(memoryStream.ToArray());
			memoryStream.Close();
			return ComplexObjectPrefix + output;
		}

		public static object Instantiate(this string input)
		{
			if (string.IsNullOrEmpty(input)) return input;

			if (input.StartsWith(ComplexObjectPrefix))
			{
				input = input.Substring(ComplexObjectPrefix.Length);
				byte[] buffer = Convert.FromBase64String(input);
				BinaryFormatter formatter = new BinaryFormatter();
				MemoryStream memoryStream = new MemoryStream(buffer);
				memoryStream.Seek(0, SeekOrigin.Begin);
				object output = formatter.Deserialize(memoryStream);
				memoryStream.Close();
				return output;
			}
			return input;
		}
		#endregion

		#region compare
		public static bool Matches(this string src, string tgt)
		{
			if (src == null && tgt == null) return true;
			if (src != null && tgt != null)
				return src.Equals(tgt, StringComparison.OrdinalIgnoreCase);
			return false;
		}

		public static bool EqualsCollection(this IList<string> srcCollection, IList<string> tgtCollection)
		{
			var srcOnly = srcCollection.Except(tgtCollection).Count();
			var tgtOnly = tgtCollection.Except(srcCollection).Count();
			return srcOnly == 0 && tgtOnly == 0;
		}

		public static bool EqualsCollection<T>(this IList<T> srcCollection, IList<T> tgtCollection, Func<T, string> fieldEvaluation)
		{
			var srcOnly = srcCollection.SkipWhile(s => tgtCollection.Any(t => fieldEvaluation(t).Matches(fieldEvaluation(s)))).Count();
			var tgtOnly = tgtCollection.SkipWhile(t => srcCollection.Any(s => fieldEvaluation(s).Matches(fieldEvaluation(t)))).Count();
			return srcOnly == 0 && tgtOnly == 0;
		}
		#endregion

		#region defaults

		public static object DefaultValue(this Type type)
		{
			return (new DefaultHelperClass()).GetDefaultValue(type);
		}
		#endregion
	}

	public static class StringBuilderExtension
	{
		public static void Replace(this StringBuilder buffer, int position, string value)
		{
			if (string.IsNullOrEmpty(value) == false && position + value.Length <= buffer.Length)
			{
				var replacement = new string(' ', value.Length);
				buffer.Replace(replacement, value, position, value.Length);
			}
		}
	}

	internal class DefaultHelperClass
	{
		public object GetDefaultValue(Type t)
		{
			return this.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(this, null);
		}

		public T GetDefaultGeneric<T>()
		{
			return default(T);
		}
	}
}
