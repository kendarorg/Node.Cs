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
using System.IO;
using Http.Renderer.Razor.Utils;

namespace Http.Renderer.Razor.Integration
{
	public interface IRazorTemplate
	{
		List<BufferItem> Buffer { get; }
		
		#region MVC complient
		string Layout { get; set; }
		dynamic ViewBag { get; }
		RawString RenderBody();

		// you may skip the layout using the parameter skipLayoutwhich can be handy 
		// when rendering template partials (controls)
		RawString RenderPage(string name, object model = null, bool skipLayout = false);

		RawString RenderSection(string sectionName, bool required = false);
		bool IsSectionDefined(string sectionName);
		#endregion

		#region Added through GeneratedClassContext
		void DefineSection(string name, Action action);
		void WriteLiteralTo(TextWriter writer, object value);
		void WriteTo(TextWriter writer, object value);
		void Write(object value);
		void WriteLiteral(object value);
		void Execute();
		#endregion
	}
}