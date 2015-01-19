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


namespace Http.Shared.Authorization
{
	public interface IUser
	{
		int UserId { get; }
		string UserName { get; }
		bool ChangePassword(string oldPassword, string newPassword);
		void SetRoles(params string[] roleNames);
	}
	public class NullAuthenticationDataProvider :  IAuthenticationDataProvider
	{
		static NullAuthenticationDataProvider _instance = new NullAuthenticationDataProvider();
		public static IAuthenticationDataProvider Instance
		{
			get
			{
				return _instance;
			}
		}
		private NullAuthenticationDataProvider()
		{

		}
		public bool IsUserAuthorized(string user, string password)
		{
			return false;
		}

		public IUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out AuthenticationCreateStatus status)
		{
			status = AuthenticationCreateStatus.ProviderError;
			return null;
		}

		public bool IsUserPresent(string userName)
		{
			return false;
		}

		public IUser GetUser(string userName, bool isOnLine)
		{
			return null;
		}

		public void AddUsersToRoles(string[] userNames, string[] roleNames)
		{

		}

		public string[] GetUserRoles(string p)
		{
			return new string[0];
		}

		public bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			return true;
		}
	}
	public interface IAuthenticationDataProviderFactory
	{
		IAuthenticationDataProvider CreateAuthenticationDataProvider();
	}
	public interface IAuthenticationDataProvider
	{
		bool IsUserAuthorized(string user, string password);
		IUser CreateUser(string username, string password, string email, string passwordQuestion,
		string passwordAnswer, bool isApproved, object providerUserKey, out AuthenticationCreateStatus status);
		bool IsUserPresent(string userName);
		IUser GetUser(string userName, bool isOnLine);
		void AddUsersToRoles(string[] userNames, string[] roleNames);
		string[] GetUserRoles(string p);
		bool DeleteUser(string username, bool deleteAllRelatedData);
	}

	public enum AuthenticationCreateStatus
	{
		Success,
		DuplicateUserName,
		DuplicateEmail,
		InvalidPassword,
		InvalidAnswer,
		InvalidEmail,
		InvalidQuestion,
		InvalidUserName,
		ProviderError,
		UserRejected
	}
}
