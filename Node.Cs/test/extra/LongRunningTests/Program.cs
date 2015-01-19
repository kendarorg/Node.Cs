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


using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LongRunningTests
{
	class Program
	{
		static void Main(string[] args)
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes().OrderBy(a => a.Namespace + "." + a.Name))
			{
				if (IsTestClass(type))
				{
					try
					{
						ExecuteTests(type);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						Debug.WriteLine(ex);
					}
				}
			}
		}

		private static IEnumerable<MethodInfo> MethodsWithAttribute<T>(Type type)
		{
			foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
			{
				if (Attribute.IsDefined(method, typeof(T)))
				{
					yield return method;
				}
			}
		}

		private static bool IsTestClass(Type type)
		{

			return MethodsWithAttribute<TestMethodAttribute>(type).Any();
		}

		private static void ExecuteTests(Type type)
		{
			Console.WriteLine("Executing: {0}.{1}", type.Namespace, type.Name);
			var testContext = new LongRunningTestContext();
			var classInstance = Activator.CreateInstance(type);
			var classInitialize = MethodsWithAttribute<ClassInitializeAttribute>(type).FirstOrDefault();
			if (classInitialize != null)
			{
				var property = type.GetProperty("TestContext");
				property.GetSetMethod().Invoke(classInstance, new object[] { testContext });
				classInitialize.Invoke(classInstance, new object[] { testContext });
			}

			try
			{
				foreach (var testMethod in MethodsWithAttribute<TestMethodAttribute>(type))
				{
					InvokeTestMethod(type, classInstance, testMethod);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Debug.WriteLine(ex);
			}
			finally
			{
				var classCleanup = MethodsWithAttribute<ClassCleanupAttribute>(type).FirstOrDefault();
				if (classCleanup != null)
				{
					classCleanup.Invoke(classInstance, new object[] { testContext });
				}
			}
		}

		private static void InvokeTestMethod(Type type,object classInstance, MethodInfo testMethod)
		{
			Console.WriteLine("\tExecuting Test {0}", testMethod.Name);
			var testInitialize = MethodsWithAttribute<TestInitializeAttribute>(type).FirstOrDefault();
			if (testInitialize != null)
			{
				testInitialize.Invoke(classInstance, new object[] { });
			}
			try
			{
				testMethod.Invoke(classInstance, new object[] { });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Debug.WriteLine(ex);
			}
			finally
			{
				var testCleanup = MethodsWithAttribute<TestCleanupAttribute>(type).FirstOrDefault();
				if (testCleanup != null)
				{
					testCleanup.Invoke(classInstance, new object[] { });
				}
			}
		}
	}
}
