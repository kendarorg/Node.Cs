using System.Linq;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
	public class ParameterInfoSpec
	{
		internal ParameterInfoSpec(bool isParamArray = false, bool hasDefault = false, object defaultValue = null)
		{
			IsParamArray = isParamArray;
			HasDefault = hasDefault;
			DefaultValue = defaultValue;
		}
		public bool IsParamArray { get; private set; }
		public bool HasDefault { get; private set; }
		public object DefaultValue { get; private set; }
	}

	public static class ParameterInfoExtension
	{
		public static ParameterInfoSpec LoadSpecifications(this ParameterInfo parameter)
		{
			var customAttribute = (ParamArrayAttribute)parameter
					.GetCustomAttributes(typeof(ParamArrayAttribute), true)
					.FirstOrDefault();
			var isParamArray = customAttribute != null;
			var optionalAtttribute = (OptionalAttribute)parameter
					.GetCustomAttributes(typeof(OptionalAttribute), true)
					.FirstOrDefault();
			var hasDefault = optionalAtttribute != null;
			object defaultValue = null;
			if (hasDefault)
			{
				defaultValue = parameter.DefaultValue;
			}
			return new ParameterInfoSpec(isParamArray, hasDefault, defaultValue);
		}
	}
}
