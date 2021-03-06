// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Node.Cs.Lib.Settings;

namespace Node.Cs.Lib.Utils
{
	public static class JsonDataService
	{

		public static object _lockObject = new object();

		public static int ReserveId(string dataName)
		{
			lock (_lockObject)
			{


				var fileName = Path.Combine(NodeCsSettings.Settings.Paths.DataDir, dataName + ".Id.txt");
				if (!File.Exists(fileName))
				{
					File.WriteAllText(fileName, "1");
					return 1;
				}

				var data = File.ReadAllText(fileName).Trim();
				var id = Int32.Parse(data);
				id++;
				// ReSharper disable once SpecifyACultureInStringConversionExplicitly
				File.WriteAllText(fileName, id.ToString());
				return id;
			}
		}

		public static void SaveData(string dataName, object data, int id)
		{
			var dirName = Path.Combine(NodeCsSettings.Settings.Paths.DataDir, dataName);
			Directory.CreateDirectory(dirName);
			var fileName = Path.Combine(dirName, id + ".json");
			var fileData = JsonConvert.SerializeObject(data);
			File.WriteAllText(fileName, fileData);
		}

		public static IEnumerable<T> ReadData<T>(string dataName)
		{
			var dirName = Path.Combine(NodeCsSettings.Settings.Paths.DataDir, dataName);
			if (!Directory.Exists(dirName)) yield break;

			foreach (var file in Directory.GetFiles(dirName, "*.json"))
			{
				var text = File.ReadAllText(file);
				yield return JsonConvert.DeserializeObject<T>(text);
			}
		}
	}
}