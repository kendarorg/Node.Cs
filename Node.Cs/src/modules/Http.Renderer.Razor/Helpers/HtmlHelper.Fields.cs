// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ClassWrapper;
using Http.Renderer.Razor.Utils;
using NodeCs.Shared.Attributes;
using NodeCs.Shared.Attributes.Validation;

namespace Http.Renderer.Razor.Helpers
{
	public partial class HtmlHelper<T>
	{
		private ItemData GetItemData<TK>(Expression<Func<TK>> expression, bool pValue = false, bool mValue = false)
		{
			var result = new ItemData
									 {
										 MainType = Lambda.GetObjectType(expression),
										 PropertyName = Lambda.GetPropertyName(expression)
									 };

			if (_classWrapper != null && _classWrapper.Instance.GetType() == result.MainType)
			{
				result.PropertyAttributes = _wrapperDescriptor.GetProperty(result.PropertyName).Attributes.ToArray();
				if (pValue)
				{
					result.PropertyValue = _classWrapper.GetObject(result.PropertyName);
				}
			}
			else
			{
				result.PropertyAttributes = Lambda.GetCustomAttribues(expression).Select(a => (Attribute)a);
				if (pValue)
				{
					result.PropertyValue = Lambda.GetPropertyValue(expression);
				}
			}
			if (mValue)
			{
				result.MainValue = Lambda.GetObject(expression);
			}

			return result;
		}

		public RawString LabelFor<TK>(Expression<Func<TK>> propertyLambda, string label = null)
		{
			if (label != null) return new RawString(label);
			var itemData = GetItemData(propertyLambda);

			foreach (var attr in itemData.PropertyAttributes)
			{
				var disp = attr as DisplayAttribute;
				if (disp != null)
				{
					return new RawString(disp.Name);
				}
			}
			return new RawString(itemData.PropertyName);
		}

		private RawString InputTypeFor<TK>(string inputType, Expression<Func<TK>> propertyLambda, string value = null)
		{
			var itemData = GetItemData(propertyLambda, true);
			if (value == null)
			{
				value = "";
				if (itemData.PropertyValue != null) value = itemData.PropertyValue.ToString();
			}
			var stringLengthAttribute =
				(StringLengthAttribute)itemData.PropertyAttributes.FirstOrDefault(a => a.GetType() == typeof(StringLengthAttribute));

			if (stringLengthAttribute == null)
				return InputTypeFor(inputType, itemData.PropertyName, value);
			return InputTypeFor(inputType, itemData.PropertyName, value, stringLengthAttribute);
		}

		public RawString CheckBoxFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			var itemData = GetItemData(propertyLambda, true);
			bool value = false;
			if (itemData.PropertyValue != null)
			{
				value = (bool)itemData.PropertyValue;
			}
			var hiddenText = HiddenFor(itemData.PropertyName, "false");
			var checkBox = InputTypeFor("checkbox", propertyLambda, value ? "true" : "");
			return new RawString(hiddenText.ToString() + checkBox.ToString());
		}

		public RawString DisplayFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			var itemData = GetItemData(propertyLambda, true);
			var value = itemData.PropertyValue ?? string.Empty;
			return new RawString(value.ToString());
		}


		public RawString CheckBoxFor(string id)
		{
			var hiddenText = HiddenFor(id, "false");
			var checkBox = InputTypeFor("checkbox", id);
			return new RawString(hiddenText.ToString() + checkBox.ToString());
		}

		private RawString InputTypeFor(string inputType, string name, string value = "", StringLengthAttribute attr = null)
		{
			var partial = new Dictionary<string, object>
			              {
				              {"type", inputType},
				              {"id", name},
				              {"name", name},
				              {"value", value}
			              };
			if (attr != null && attr.MaximumLength > 0)
			{
				partial.Add("maxlength", attr.MaximumLength);
			}
			var result = TagBuilder.StartTag("input", partial, true);
			return new RawString(result);
		}



		public RawString LabelFor(string label)
		{
			return new RawString(label);
		}

		public RawString TextBoxFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			return InputTypeFor("text", propertyLambda);
		}

		public RawString TextBoxFor(string id, string value = "")
		{
			return InputTypeFor("text", id, value);
		}



		public RawString HiddenFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			return InputTypeFor("hidden", propertyLambda);
		}

		public RawString HiddenFor(string id, string value = "")
		{
			return InputTypeFor("hidden", id, value);
		}

		public RawString PasswordFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			return InputTypeFor("password", propertyLambda);
		}

		public RawString PasswordFor(string id, string value = "")
		{
			return InputTypeFor("password", id, value);
		}


		public RawString FileFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			return InputTypeFor("file", propertyLambda);
		}

		public RawString FileFor(string id)
		{
			return InputTypeFor("file", id);
		}


		public RawString ValidationSummary(bool whoKnows, string message = null)
		{
			if (ViewContext.ModelState.IsValid) return new RawString("");
			var errors = ViewContext.ModelState.GetErrors("");
			var result = message ?? string.Empty;
			if (errors.Count > 0)
			{
				result += "<ul>";
				foreach (var error in errors)
				{
					result += "<li>" + error + "</li>";
				}
				result += "</ul>";
			}
			return new RawString(result);
		}

		public RawString ValidationMessageFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			if (ViewContext.ModelState.IsValid) return new RawString("");
			var name = Lambda.GetPropertyName(propertyLambda);

			var errors = ViewContext.ModelState.GetErrors(name);
			if (errors.Count == 0) return new RawString("");
			var result = "";
			if (errors.Count > 1)
			{
				result += "<ul>";
				foreach (var error in errors)
				{
					result += "<li>" + error + "</li>";
				}
				result += "</ul>";
			}
			else
			{
				result = errors[0];
			}
			return new RawString(result);
		}

		public RawString EditorFor<TK>(Expression<Func<TK>> propertyLambda)
		{
			var itemData = GetItemData(propertyLambda, true);
			IEnumerable<Attribute> attributes = null;
			attributes = itemData.PropertyAttributes;
			var dataType = DataType.Text;


			foreach (var attr in attributes)
			{
				var disp = attr as DataTypeAttribute;
				if (disp != null)
				{
					dataType = disp.DataType;
				}
				else
				{
					var sca = attr as ScaffoldColumnAttribute;
					if (sca != null)
					{
						if (!sca.Scaffold)
						{
							return HiddenFor(propertyLambda);
						}
					}
				}
			}

			var label = LabelFor(propertyLambda);
			switch (dataType)
			{
				case (DataType.Upload):
					return Combine(label, FileFor(propertyLambda));
				case (DataType.Password):
					return Combine(label, PasswordFor(propertyLambda));
				case (DataType.Html):
				case (DataType.Url):
				case (DataType.EmailAddress):
				case (DataType.ImageUrl):
				case (DataType.MultilineText):
					throw new NotImplementedException("DataType");
				default:
					return Combine(label, TextBoxFor(propertyLambda));
			}
		}

		private RawString EditorFor(PropertyWrapperDescriptor wrapperDescriptor)
		{
			string name = wrapperDescriptor.Name;
			IEnumerable<Attribute> attributes = wrapperDescriptor.Attributes;
			var value = _classWrapper.GetObject(name);
			string stringValue = value == null ? string.Empty : value.ToString();
			var dataType = DataType.Text;

			foreach (var attr in attributes)
			{
				var disp = attr as DataTypeAttribute;
				if (disp != null)
				{
					dataType = disp.DataType;
				}
				else
				{
					var sca = attr as ScaffoldColumnAttribute;
					if (sca != null)
					{
						if (!sca.Scaffold)
						{
							return HiddenFor(name, stringValue);
						}
					}
				}
			}

			var label = "<div class='editor-label'>" + LabelFor(name) + "</div>";
			const string startField = "<div class='editor-field'>";
			const string endField = "</div>";
			switch (dataType)
			{
				case (DataType.Upload):
					return Combine(label, startField + FileFor(name) + endField);
				case (DataType.Password):
					return Combine(label, startField + PasswordFor(name, stringValue) + endField);
				case (DataType.Html):
				case (DataType.Url):
				case (DataType.ImageUrl):
				case (DataType.MultilineText):
					throw new NotImplementedException("DataType");
				case (DataType.EmailAddress):
				default:
					return Combine(label, startField + TextBoxFor(name, stringValue) + endField);
			}
		}

		private RawString Combine(string label, string rest)
		{
			return new RawString(label + rest);
		}

		private RawString Combine(params RawString[] raws)
		{
			var stringResult = string.Empty;
			foreach (var raw in raws)
			{
				stringResult += "\r\n" + raw;
			}
			return new RawString(stringResult);
		}

		public RawString DropDownList(string id, object selectedValue = null)
		{
			var mem = Lambda.GetProperty(ViewBag, id);
			var sl = mem as SelectList;
			if (sl == null) throw new Exception("Missing SelectList for id " + id);
			string selTostring = null;
			if (selectedValue != null)
			{
				selTostring = selectedValue.ToString();
			}
			var start = TagBuilder.StartTag("select", new Dictionary<string, object> { { "id", id }, { "name", id } });
			var options = string.Empty;
			var end = TagBuilder.EndTag("select");

			var starting = true;
			MethodInfo pinfovalue = null;
			MethodInfo pinfotext = null;
			foreach (var item in sl.Items)
			{
				var selected = false;
				if (starting)
				{
					pinfovalue = item.GetType().GetProperty(sl.DataValueField).GetGetMethod();
					pinfotext = pinfovalue;
					if (sl.DataTextField != sl.DataValueField)
					{
						pinfotext = item.GetType().GetProperty(sl.DataTextField).GetGetMethod();
					}
					starting = false;
				}
				var value = pinfovalue.Invoke(item, new object[] { }).ToString();
				// ReSharper disable once RedundantToStringCall
				var text = value.ToString();
				if (sl.DataTextField != sl.DataValueField)
				{
					text = pinfotext.Invoke(item, new object[] { }).ToString();
				}
				if (selTostring != null && selTostring == value)
				{
					selected = true;
				}
				var dict = new Dictionary<string, object>();

				if (selected)
				{
					dict.Add("selected", "selected");
				}
				dict.Add("value", value);
				options += TagBuilder.TagWithValue("option", text, dict);
			}

			return new RawString(start + options + end);
		}

		public RawString Label(string labelText)
		{
			return new RawString(labelText);
		}

		public RawString TextBox(string id)
		{
			return TextBoxFor(id);
		}
	}
}
