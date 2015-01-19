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


using NodeCs.Shared;
using System;

namespace Http.Shared.Authorization
{
	public static class WebSecurity
	{
		public static bool Login(string userName, string password, bool persistCookie = false)
		{
			if (persistCookie)
			{
				throw new NotImplementedException();
			}
			var adp = ServiceLocator.Locator.Resolve<IAuthenticationDataProvider>();

			return adp.IsUserAuthorized(userName, password);
		}

		public static void Logout()
		{
			throw new NotImplementedException();
		}

		public static void CreateUserAndAccount(string userName, string password, Object propertyValues = null, bool requireConfirmationToken = false)
		{
			if (requireConfirmationToken)
			{
				throw new NotImplementedException();
			}
			var adp = ServiceLocator.Locator.Resolve<IAuthenticationDataProvider>();
			AuthenticationCreateStatus result;
			adp.CreateUser(userName, password, string.Empty, string.Empty, Guid.NewGuid().ToString(), true, null, out result);
			if (result != AuthenticationCreateStatus.Success)
			{
				throw new MembershipCreateUserException(userName, result);
			}
		}

		public static int GetUserId(string name)
		{
			var adp = ServiceLocator.Locator.Resolve<IAuthenticationDataProvider>();
			var user = adp.GetUser(name, false);
			return user.UserId;
		}

		public static bool ChangePassword(string name, string oldPassword, string newPassword)
		{
			var adp = ServiceLocator.Locator.Resolve<IAuthenticationDataProvider>();
			var user = adp.GetUser(name, false);
			return user.ChangePassword(oldPassword, newPassword);
		}

		public static void CreateAccount(string name, string newPassword, bool requireConfirmationToken = false)
		{
			CreateUserAndAccount(name, newPassword, requireConfirmationToken);
		}
	}
}
