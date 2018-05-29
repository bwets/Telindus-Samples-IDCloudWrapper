using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IDCloudWrapper.Model
{
	
	public class Results: Dictionary<string, Property>
	{
		private const string DefaultTrueBoolValue = "OK";

		public Guid Reference { get; set; }
		public ResultType Type { get; }

		public Results() : base(StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public void AddField(Property property)
		{
			this[property.Code] = property;
		}

		public string   GetString(string code) => this[code]?.Value;
		public bool     GetBool(string   code) => this[code]?.Value == DefaultTrueBoolValue;
		public float    GetFloat(string  code) => System.Convert.ToSingle(this[code]?.Value, CultureInfo.InvariantCulture);
		public string[] GetArray(string  code) => this.Where(x => x.Value.Code == code).Select(x => x.Value.Value).ToArray();

	}
}