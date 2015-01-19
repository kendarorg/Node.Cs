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
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Http.Renderer.Razor.Utils
{
	public class ItemData
	{
		public string PropertyName;
		public object PropertyValue;
		public IEnumerable<Attribute> PropertyAttributes;
		public object MainValue;
		public Type MainType;
	}

	public class LambdaHelper
	{
		public LambdaHelper()
		{
			_memberExpressions = new Dictionary<Type, FieldInfo>();
			_memberAttributes = new Dictionary<PropertyInfo, object[]>();
		}

		public Dictionary<Type, FieldInfo> _memberExpressions;
		private readonly Dictionary<PropertyInfo, object[]> _memberAttributes;

		public object GetPropertyValue<T>(Expression<Func<T>> e)
		{
			return e.Compile()();
		}

		public object GetObject<T>(Expression<Func<T>> e)
		{
			var me = (MemberExpression)((MemberExpression)e.Body).Expression;
			var ce = (ConstantExpression)me.Expression;
			var valueType = ce.Value.GetType();
			if (!_memberExpressions.ContainsKey(valueType))
			{
				var fieldInfo = ce.Value.GetType().GetField(me.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				_memberExpressions.Add(valueType, fieldInfo);
			}

			return _memberExpressions[valueType].GetValue(ce.Value);
		}

		public Type GetObjectType<T>(Expression<Func<T>> e)
		{
			var member = (MemberExpression)e.Body;
			Expression strExpr = member.Expression;
			return strExpr.Type;
		}

		public string GetPropertyName<T>(Expression<Func<T>> e)
		{
			var member = (MemberExpression)e.Body;
			return member.Member.Name;
		}

		public PropertyInfo GetProperty<T>(Expression<Func<T>> e)
		{
			var member = (MemberExpression)e.Body;
			return member.Member as PropertyInfo;
		}
		/*
		public PropertyInfo Property<T>(Expression<Func<T>> e)
		{
			var member = e.Body as MemberExpression;

			// Check if there is a cast to object in first position
			if (member == null)
			{
				var unary = e.Body as UnaryExpression;
				if (unary != null)
				{
					member = unary.Operand as MemberExpression;
				}
			}
			return member.Member as PropertyInfo;
		}*/

		public object[] GetCustomAttribues<TK>(Expression<Func<TK>> expression)
		{
			var pinfo = GetProperty(expression);
			if (!_memberAttributes.ContainsKey(pinfo))
			{
				_memberAttributes.Add(pinfo, pinfo.GetCustomAttributes(true));
			}
			return _memberAttributes[pinfo];
		}

		public object GetProperty(object o, string member)
		{
			if (o == null) throw new ArgumentNullException("o");
			if (member == null) throw new ArgumentNullException("member");
			Type scope = o.GetType();
			IDynamicMetaObjectProvider provider = o as IDynamicMetaObjectProvider;
			if (provider != null)
			{
				ParameterExpression param = Expression.Parameter(typeof(object));
				DynamicMetaObject mobj = provider.GetMetaObject(param);
				GetMemberBinder binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, member, scope, new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(0, null) });
				DynamicMetaObject ret = mobj.BindGetMember(binder);
				BlockExpression final = Expression.Block(
						Expression.Label(CallSiteBinder.UpdateLabel),
						ret.Expression
				);
				LambdaExpression lambda = Expression.Lambda(final, param);
				Delegate del = lambda.Compile();
				return del.DynamicInvoke(o);
			}
			else
			{
				return o.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance).GetValue(o, null);
			}
		}
	}
}
