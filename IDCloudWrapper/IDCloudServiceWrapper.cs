using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IDCloudWrapper.Model;

namespace IDCloudWrapper
{
	public class IDCloudServiceWrapper
	{
		public IDCloudServiceWrapper(string url, string company, string user, string password)
		{
			this.Url = url;
			this.Company = company;
			this.User = user;
			this.Password = password;
		}

		//"https://demoidcloud.icarvision.com:50061/iCarSAAS.WsPublic/WsDocument.asmx"
		public string Url { get; set; }
		public string Company { get; set; }
		public string User { get; set; }
		public string Password { get; set; }

		private Guid Prepare(StringBuilder s)
		{
			var reference = Guid.NewGuid();
			s.Replace("{company}", this.Company);
			s.Replace("{user}", this.User);
			s.Replace("{password}", this.Password);
			s.Replace("{reference}", reference.ToString());
			return reference;
		}

		private void Apply(StringBuilder s, int imageIndex, byte[] imageBytes, string imageType)
		{
			var imageData = Convert.ToBase64String(imageBytes);

			s.Replace("{image" + imageIndex + "Data}", imageData);
			s.Replace("{image" + imageIndex + "Type}", imageType);
		}

		private async Task<string> Execute(string requestXml, string url)
		{
			var client = new HttpClient();
			var content = new StringContent(requestXml);

			content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/soap+xml; charset=utf-8");
			content.Headers.ContentLength = requestXml.Length;
			var response = await client.PostAsync(url, content);
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<Results> Execute(byte[] image1Bytes, string image1Type, byte[] image2Bytes, string image2Type)
		{
			var template = await Template.For2Images();
			var builder = new StringBuilder(template);
			var reference = this.Prepare(builder);
			this.Apply(builder, 1, image1Bytes, image1Type);
			this.Apply(builder, 2, image2Bytes, image2Type);
			var xml = builder.ToString();
			var responseXml = await this.Execute(xml, this.Url);
			return ResultsParser.Parse(reference, responseXml);
		}

		public async Task<Results> Execute(byte[] imageBytes, string imageType)
		{
			var template = await Template.For1Image();
			var builder = new StringBuilder(template);
			var reference = this.Prepare(builder);
			this.Apply(builder, 1, imageBytes, imageType);
			var xml = builder.ToString();
			var responseXml = await this.Execute(xml, this.Url);
			return ResultsParser.Parse(reference,responseXml);
		}
	}
}
