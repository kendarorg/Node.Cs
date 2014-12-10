//------------------------------------------------------------------------------
// <copyright file="ExceptionAssert.cs" company="Campari Software">
//    Copyright (C) 2001-2009 Campari Software. All rights reserved.
// </copyright>
//
// <disclaimer>
//    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED
//    TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//    PARTICULAR PURPOSE.
// </disclaimer>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Node.Cs.Test
{

	/// <summary>
	/// Contains assertion types that are not provided with the standard MSTest assertions.
	/// </summary>
	[DebuggerStepThrough]
	[DebuggerNonUserCode]
	public static class ExceptionAssert
	{
		#region Throws<TException>(Action action)

		/// <summary>
		/// Checks to make sure that the input delegate throws a exception of type exceptionType.
		/// </summary>
		/// <typeparam name="TException">The type of exception expected.</typeparam>
		/// <param name="action">The action to execute to generate the exception.</param>
		public static void Throws<TException>(Action action) where TException : Exception
		{
			TException ex;
			Throws(action, out ex);
		}

		public static void Throws<TException>(Action action, out TException result) where TException : Exception
		{
			result = null;
			try
			{
				action();
			}
			catch (Exception ex)
			{
				result = ex as TException;
				Assert.IsInstanceOfType(ex, typeof(TException), "Expected exception was not thrown. ");
				return;
			}

			Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
		}
		#endregion

		#region Throws<TException>(string expectedMessage, Action action)

		/// <summary>
		/// Checks to make sure that the input delegate throws a exception of type exceptionType.
		/// </summary>
		/// <typeparam name="TException">The type of exception expected.</typeparam>
		/// <param name="expectedMessage">The expected message text.</param>
		/// <param name="action">The action to execute to generate the exception.</param>
		/// <param name="result"></param>
		public static void Throws<TException>(string expectedMessage, Action action,out TException result) where TException : Exception
		{
			result = null;
			try
			{
				action();
			}
			catch (Exception ex)
			{
				result = ex as TException;
				Assert.IsInstanceOfType(ex, typeof(TException), "Expected exception was not thrown. ");
				Assert.AreEqual(expectedMessage, ex.Message, "Expected exception with a message of '" + expectedMessage + "' but exception with message of '" + ex.Message + "' was thrown instead.");
				return;
			}

			Assert.Fail("Expected exception of type " + typeof(TException) + " but no exception was thrown.");
		}

		public static void Throws<TException>(string expectedMessage, Action action) where TException : Exception
		{
			TException ex;
			Throws(expectedMessage,action,out ex);
		}
		#endregion
	}
}
