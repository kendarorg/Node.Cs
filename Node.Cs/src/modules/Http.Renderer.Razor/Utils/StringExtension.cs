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


#region  Microsoft Public License
/* This code is part of Xipton.Razor v3.0
 * (c) Jaap Lamfers, 2013 - jaap.lamfers@xipton.net
 * Licensed under the Microsoft Public License (MS-PL) http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 */
#endregion

using System;
using System.IO;

namespace Http.Renderer.Razor.Utils
{
    public static class StringExtension
    {

        public static string FormatWith(this string format, params object[] args)
        {
            return format == null ? null : string.Format(format, args);
        }

        public static bool NullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string EmptyAsNull(this string value) {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static bool HasVirtualRootOperator(this string path)
        {
            return path != null && path.StartsWith("~");
        }

        public static string RemoveRoot(this string path)
        {
            if (path.NullOrEmpty()) return path;
            if (path[0] == '~') path = path.Substring(1);
            return path.TrimStart('\\').TrimStart('/');
        }

        public static bool IsAbsoluteVirtualPath(string path)
        {
            return path != null && (path.Contains(":") || path.StartsWith("/") || path.StartsWith("\\"));
        }

        public static string MakeAbsoluteDirectoryPath(this string path){
            var baseDir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            if (baseDir == null)
                return path;
            return Path.Combine(baseDir, path ?? ".");
        }

        public static string HtmlEncode(this string value)
        {
            return value == null ? null : System.Net.WebUtility.HtmlEncode(value);
        }

        internal static bool IsFileName(this string path) {
            return path != null && File.Exists(path);
        }
        internal static bool IsXmlContent(this string content) {
            return content != null && content.TrimStart().StartsWith("<");
        }
        internal static bool IsUrl(this string value) {
            return value != null && (value.HasVirtualRootOperator() || value.StartsWith("/"));
        }

    }
}