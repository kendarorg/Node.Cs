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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Microsoft.CSharp;

namespace Http.Renderer.Razor.Integration
{
	public class Compiler
	{
		public static ExpandoObject ForceCore;
		static Compiler()
		{
			ForceCore = new ExpandoObject();
		}
		private static string TypeToString(Type type)
		{
			var typeName = type.Name;
			var thinghy = typeName.IndexOf('`');
			if (thinghy > 0)
			{
				typeName = typeName.Substring(0, thinghy);
			}
			var res = type.Namespace + "." + typeName;
			if (type.IsGenericType)
			{
				res += "<";
				var args = type.GetGenericArguments();
				for (int i = 0; i < args.Length; i++)
				{
					if (i > 0) res += ", ";
					res += TypeToString(args[i]);
				}
				res += ">";
			}
			return res;
		}

		private static GeneratorResults GenerateCode(RazorTemplateEntry entry)
		{
			var host = new NodeRazorHost(new CSharpRazorCodeLanguage());

			host.DefaultBaseClass = string.Format("Http.Renderer.Razor.Integration.RazorTemplateBase<{0}>", TypeToString(entry.ModelType));
			host.DefaultNamespace = "Http.Renderer.Razor.Integration";
			host.DefaultClassName = entry.TemplateName + "Template";
			host.NamespaceImports.Add("System");
			host.NamespaceImports.Add("System.Dynamic");
			GeneratorResults razorResult = null;
			using (TextReader reader = new StringReader(entry.TemplateString))
			{
				razorResult = new RazorTemplateEngine(host).GenerateCode(reader);
			}
			return razorResult;
		}

		private static CompilerParameters BuildCompilerParameters()
		{
			var @params = new CompilerParameters();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.ManifestModule.Name != "<In Memory Module>")
					@params.ReferencedAssemblies.Add(assembly.Location);
			}
			@params.GenerateInMemory = true;
#if DEBUG
			@params.IncludeDebugInformation = true;
#else
			@params.IncludeDebugInformation = false;
#endif
			@params.GenerateExecutable = false;
			@params.CompilerOptions = "/target:library /optimize";
			return @params;
		}

		public static Assembly Compile(IEnumerable<RazorTemplateEntry> entries)
		{
			var builder = new StringBuilder();
			var codeProvider = new CSharpCodeProvider();
			using (var writer = new StringWriter(builder))
			{
				var entriesArray = entries.ToArray();
				for (int index = 0; index < entriesArray.Length; index++)
				{
					var razorTemplateEntry = entriesArray[index];
					if (razorTemplateEntry.TemplateType == null)
					{
						var generatorResults = GenerateCode(razorTemplateEntry);
						codeProvider.GenerateCodeFromCompileUnit(generatorResults.GeneratedCode, writer, new CodeGeneratorOptions());
					}
				}
			}

			var result = codeProvider.CompileAssemblyFromSource(BuildCompilerParameters(), new[] { builder.ToString() });
			if (result.Errors != null && result.Errors.Count > 0)
				throw new TemplateCompileException(result.Errors, builder.ToString());

			return result.CompiledAssembly;
		}
	}
}