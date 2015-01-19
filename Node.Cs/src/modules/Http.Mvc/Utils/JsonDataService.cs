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


namespace HttpMvc.Utils
{
	public static class JsonDataService
	{
		/*
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
		}*/
	}
}