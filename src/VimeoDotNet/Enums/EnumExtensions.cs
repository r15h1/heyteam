﻿using System;
using System.Reflection;

namespace VimeoDotNet.Enums
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
	internal class ParameterValueAttribute : Attribute
	{
		public string TextValue { get; set; }

		public ParameterValueAttribute(string textValue)
		{
			this.TextValue = textValue;
		}
	}

    public static class EnumExtensions
	{
		public static string GetParameterValue(this Enum value)
		{
			var type = value.GetType();
			var name = Enum.GetName(type, value);
			if (name == null)
				return value.ToString().ToLower();
			var field = type.GetRuntimeField(name);
			if (field == null)
				return value.ToString().ToLower();
			var attr = field.FieldType.GetTypeInfo().GetCustomAttribute<ParameterValueAttribute>();
			return attr != null ? attr.TextValue : value.ToString().ToLower();
		}
	}
}