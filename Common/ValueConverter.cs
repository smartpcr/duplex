using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	/// <summary>
	/// boolean value chars
	/// </summary>
	public class BoolValue
	{
		/// <summary>
		/// char represents bool value flag
		/// </summary>
		public char CharValue { get; set; }

		public string StringValue { get; set; }

		/// <summary>
		/// return bool value from char
		/// </summary>
		/// <returns></returns>
		public bool GetValue()
		{
			char[] availableTrueValues = new char[] { 'y', 't', '1', 'Y', 'T' };
			foreach (char yesVal in availableTrueValues)
			{
				if (this.CharValue == yesVal) return true;
			}

			if (!string.IsNullOrEmpty(this.StringValue))
			{
				if (this.StringValue.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
					return true;
				if (this.StringValue.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
					return true;
				if (this.StringValue.Equals("True", StringComparison.InvariantCultureIgnoreCase))
					return true;
				if (this.StringValue.Equals("T", StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}
	}

	/// <summary>
	/// value converter
	/// </summary>
	public class ValueConverter
	{
		/// <summary>
		/// handles nullable type and type cast
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static object ChangeType(object value, Type targetType)
		{
			if (value == null || value == DBNull.Value) return null;
			if (value.GetType() == targetType) return value;
			if (targetType.IsEnum && value is int)
			{
				return Enum.Parse(targetType, value.ToString());
			}

			try
			{
				try
				{
					return Convert.ChangeType(value, targetType);
				}
				catch
				{
					if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						if (targetType.GetGenericArguments().Length > 0 && targetType.GetGenericArguments()[0].IsEnum)
						{
							if (value.GetType() == typeof(int?))
							{
								int? intValue = (int?)value;
								if (intValue.HasValue)
								{
									TypeConverter convert = TypeDescriptor.GetConverter(targetType);
									return convert.ConvertFrom(intValue.Value + "");
								}
								else return null;
							}
							else if (value is int)
							{
								TypeConverter convert = TypeDescriptor.GetConverter(targetType);
								return convert.ConvertFrom((int)value + "");
							}
						}

						value = Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType));
					}
					return value;
				}
			}
			catch
			{
				object propValue = null;
				if (targetType == typeof(decimal) && value.GetType() == typeof(decimal?))
				{
					decimal? decValue = (decimal?)value;
					if (decValue.HasValue)
						propValue = decValue.Value;
				}
				else if (targetType == typeof(int) && value.GetType() == typeof(int?))
				{
					int? intValue = (int?)value;
					if (intValue.HasValue)
						propValue = intValue.Value;
				}
				else if (targetType == typeof(decimal?) && value.GetType() == typeof(decimal))
				{
					decimal? decValue = (decimal)value;
					propValue = decValue;
				}
				else if (targetType == typeof(int?) && value.GetType() == typeof(int))
				{
					int? intValue = (int)value;
					propValue = intValue;
				}
				else if (targetType == typeof(decimal) && value.GetType() == typeof(int))
				{
					propValue = (decimal)value;
				}
				else if (targetType == typeof(decimal?) && value.GetType() == typeof(int?))
				{
					int? intValue = (int?)value;
					if (intValue.HasValue)
						propValue = (decimal)intValue.Value;
				}
				else if (targetType == typeof(int?) && value.GetType() == typeof(decimal?))
				{
					decimal? decValue = (decimal?)value;
					if (decValue.HasValue)
						propValue = int.Parse(decValue.Value.ToString());
				}
				else if (targetType == typeof(int) && value.GetType() == typeof(decimal))
				{
					propValue = int.Parse(value.ToString());
				}
				else if (targetType == typeof(bool) && value.GetType() == typeof(char))
				{
					char c = (char)value;
					BoolValue boolVal = new BoolValue { CharValue = c };
					propValue = boolVal.GetValue();
				}
				else if (targetType == typeof(bool) && value.GetType() == typeof(string))
				{
					string c = (string)value;
					BoolValue boolVal = new BoolValue { StringValue = c };
					propValue = boolVal.GetValue();
				}
				else if (targetType == typeof(char) && value.GetType() == typeof(bool))
				{
					bool boolValue = (bool)value;
					propValue = boolValue ? 'Y' : 'N';
				}
				else if (targetType == typeof(string))
				{
					propValue = value.ToString();
				}
				else if (targetType == typeof(char) && value.GetType() == typeof(string))
				{
					string strValue = value.ToString();
					if (string.IsNullOrEmpty(strValue)) return null;
					else if (strValue.Length == 1) propValue = strValue[0];
				}
				else
				{
					try
					{
						propValue = Convert.ChangeType(value, targetType);
					}
					catch (Exception ex)
					{
						string errorMessage =
							string.Format("Unable to cast value {0} to type {1}",
										  value, targetType.Name);
						throw new Exception(errorMessage, ex);
					}
				}

				return propValue;
			}
		}

		/// <summary>
		/// type cast
		/// </summary>
		/// <param name="prop"></param>
		/// <param name="target"></param>
		/// <param name="value"></param>
		public static void SetValue(PropertyInfo prop, object target, object value)
		{
			if (value == null || value == DBNull.Value) return;
			if (prop.PropertyType == value.GetType())
			{
				prop.SetValue(target, value, null);
				return;
			}
			if (prop.PropertyType.IsEnum && value is int)
			{
				value = Enum.Parse(prop.PropertyType, value.ToString());
				prop.SetValue(target, value, null);
			}
			try
			{
				if (prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					value = Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType));
				}
				prop.SetValue(target, value, null);
				return;
			}
			catch
			{
				object propValue = null;
				if (prop.PropertyType == typeof(decimal) && value.GetType() == typeof(decimal?))
				{
					decimal? decValue = (decimal?)value;
					if (decValue.HasValue)
						propValue = decValue.Value;
				}
				else if (prop.PropertyType == typeof(int) && value.GetType() == typeof(int?))
				{
					int? intValue = (int?)value;
					if (intValue.HasValue)
						propValue = intValue.Value;
				}
				else if (prop.PropertyType == typeof(decimal?) && value.GetType() == typeof(decimal))
				{
					decimal? decValue = (decimal)value;
					propValue = decValue;
				}
				else if (prop.PropertyType == typeof(int?) && value.GetType() == typeof(int))
				{
					int? intValue = (int)value;
					propValue = intValue;
				}
				else if (prop.PropertyType == typeof(decimal) && value.GetType() == typeof(int))
				{
					propValue = (decimal)value;
				}
				else if (prop.PropertyType == typeof(decimal?) && value.GetType() == typeof(int?))
				{
					int? intValue = (int?)value;
					if (intValue.HasValue)
						propValue = (decimal)intValue.Value;
				}
				else if (prop.PropertyType == typeof(int?) && value.GetType() == typeof(decimal?))
				{
					decimal? decValue = (decimal?)value;
					if (decValue.HasValue)
						propValue = int.Parse(decValue.Value.ToString());
				}
				else if (prop.PropertyType == typeof(int) && value.GetType() == typeof(decimal))
				{
					propValue = int.Parse(value.ToString());
				}
				else if (prop.PropertyType == typeof(bool) && value.GetType() == typeof(char))
				{
					char c = (char)value;
					BoolValue boolVal = new BoolValue { CharValue = c };
					propValue = boolVal.GetValue();
				}
				else if (prop.PropertyType == typeof(bool?) && value.GetType() == typeof(char))
				{
					char c = (char)value;
					BoolValue boolVal = new BoolValue { CharValue = c };
					propValue = boolVal.GetValue();
				}
				else if (prop.PropertyType == typeof(bool) && value.GetType() == typeof(string))
				{
					if (value.ToString().Length == 1)
					{
						char c = value.ToString()[0];
						BoolValue boolVal = new BoolValue { CharValue = c };
						propValue = boolVal.GetValue();
					}
				}
				else if (prop.PropertyType == typeof(bool?) && value.GetType() == typeof(string))
				{
					if (value.ToString().Length == 1)
					{
						char c = value.ToString()[0];
						BoolValue boolVal = new BoolValue { CharValue = c };
						propValue = boolVal.GetValue();
					}
				}
				else if (prop.PropertyType == typeof(char) && value.GetType() == typeof(bool))
				{
					bool boolValue = (bool)value;
					propValue = boolValue ? 'Y' : 'N';
				}
				else if (prop.PropertyType == typeof(string))
				{
					propValue = value.ToString();
				}
				else if (prop.PropertyType == typeof(char) && value.GetType() == typeof(string))
				{
					string strValue = value.ToString();
					if (string.IsNullOrEmpty(strValue)) return;
					else if (strValue.Length == 1) propValue = strValue[0];
				}
				else
				{
					try
					{
						propValue = Convert.ChangeType(value, prop.PropertyType);
					}
					catch (Exception ex)
					{
						string errorMessage =
							string.Format("Unable to set value {0} to property {1} for object {2}",
										  value, prop.Name, target.GetType().Name);
						throw new Exception(errorMessage, ex);
					}
				}

				if (propValue != null)
					prop.SetValue(target, propValue, null);
			}
		}
	}
}
