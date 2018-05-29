using System.Diagnostics;

namespace IDCloudWrapper.Model
{
	[DebuggerDisplay("{Code}={Value}")]	public class Property
	{
		public string Type { get; set; }
		public string Code { get; set; }
		public string Value { get; set; }
		public string Description { get; set; }

	}
}