using System.Xml.Linq;

namespace IDCloudWrapper
{
	public static class XElementExtensions
	{
		public static string Value(this XElement e, XName child)
		{
			return e.Element(child)?.Value;
		}
		public static XElement Traverse(this XContainer e, params XName[] names)
		{
			var x = e;
			foreach (var name in names)
			{
				x = x.Element(name);
				if (x == null)
					return null;
			}

			return x as XElement;
		}
	}
}