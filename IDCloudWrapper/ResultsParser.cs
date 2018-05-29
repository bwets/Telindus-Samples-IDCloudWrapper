using System;
using System.Xml.Linq;

namespace IDCloudWrapper.Model
{
	public static class ResultsParser
	{
		public static Results Parse(Guid reference, string xml)
		{
			XNamespace soap         = "http://www.w3.org/2003/05/soap-envelope";
			XNamespace icar         = "http://IdCloud.iCarVision/WS/";
			var        xCode        = icar + "Code";
			var        xDescription = icar + "Description";
			var        xValue       = icar + "Value";
			var        xType        = icar + "Type";

			var document = XDocument.Parse(xml);

			var resultElement = document.Traverse(soap + "Envelope", soap + "Body", icar + "AnalyzeDocumentV2Response", icar + "AnalyzeDocumentV2Result");
			if (resultElement == null) throw new ApplicationException("Invalid response");
			var icarResults = new Results() { Reference = reference};

			var fields = resultElement.Element(icar + "Fields");
			if (fields != null)
			{
				foreach (var field in fields.Elements(icar + "Field"))
				{
					icarResults.AddField(new Property
										 {
											 Code        = field.Value(xCode),
											 Description = field.Value(xDescription),
											 Value       = field.Value(xValue),
											 Type        = field.Value(xType)
										 });
				}
			}

			return icarResults;
		}
	}
}
