using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable once CheckNamespace


namespace System.Xml.Linq
{
	public static class XmlExtension
	{
		public static IEnumerable<XElement> DescendantsByTag(this XElement src, string tag)
		{
			return src.DescendantNodes().Where(el =>
			{
				var xe = el as XElement;
				if (xe == null) return false;
				return String.Compare(xe.Name.LocalName, tag, StringComparison.OrdinalIgnoreCase) == 0;
			}).Cast<XElement>();
		}


		public static IEnumerable<XElement> DescendantsByTag(this XDocument src, string tag)
		{
			return src.DescendantNodes().Where(el =>
			{
				var xe = el as XElement;
				if (xe == null) return false;
				return String.Compare(xe.Name.LocalName, tag, StringComparison.OrdinalIgnoreCase) == 0;
			}).Cast<XElement>();
		}

		public static XDocument LoadDocument(byte[] data)
		{
			var ms = new MemoryStream(data);
			ms.Seek(0, SeekOrigin.Begin);
			return XDocument.Load(ms);
		}
	}
}
