using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IDCloudWrapper.Model;
using Xunit;

namespace IDCloudWrapper.Tests
{
	public class Image
	{
		public byte[] Data { get; set; }
		public string Type { get; set; }

		public static Image Load(string filename)
		{
			var image = new Image { Data = File.ReadAllBytes(filename), Type = System.IO.Path.GetExtension(filename).Replace(".", "").ToLower() };
			return image;
		}

		public string Base64Data => Convert.ToBase64String(this.Data);
	}

	public class Log
	{
		public string Filename { get; set; }
		public Log(string filename)
		{
			this.Filename = filename;
		}

		public void Write(params string[] lines)
		{
			File.AppendAllLines(this.Filename,lines);

		}

		public void Write(Results results)
		{
			this.Write("Results for " + results.Reference);
			this.Write(results.Select(x=>$"\t{x.Value.Code} = {x.Value.Value}").ToArray());

		}
	}
	public class UnitTest1
	{
		public const string TangoImages = @"C:\Temp\Icar\tango\";
		public Log Log { get; }
		public UnitTest1()
		{
			this.Log = new Log(@"C:\Temp\Icar\UniTests.log");
			this.Log.Write("---------------------------------------------------------------------------------------");
			this.Log.Write("Test run: " + DateTime.Now.ToLongTimeString());
		}

		[Fact]
		public void TestResultsParsing()
		{
			var resultsXml = File.ReadAllText("results.xml");
			var results = ResultsParser.Parse(Guid.NewGuid(), resultsXml);
			this.Log.Write(results);
			Assert.NotEmpty(results);

		}

		[Theory]
		[InlineData("3780-card.jpg", "3780-photo.jpg", TangoImages)]
		[InlineData("3795-card.jpg", "3795-photo.jpg", TangoImages)]
		[InlineData("3820-card.jpg", "3820-photo.jpeg", TangoImages)]
		[InlineData("3821-card.jpg", "3821-photo.jpg", TangoImages)]
		[InlineData("3825-card.jpg", "3825-photo.jpg", TangoImages)]
		[InlineData("3846-card.jpg", "3846-photo.jpg", TangoImages)]
		[InlineData("3866-card.jpg", "3866-photo.jpg", TangoImages)]
		[InlineData("3867-card.jpg", "3867-photo.jpg", TangoImages)]
		[InlineData("3872-card.jpg", "3872-photo.jpg", TangoImages)]
		[InlineData("3883-card.jpg", "3883-photo.jpg", TangoImages)]
		[InlineData("3889-card.png", "3889-photo.png", TangoImages)]
		[InlineData("3895-card.jpg", "3895-photo.jpg", TangoImages)]
		[InlineData("3898-card.jpg", "3898-photo.jpg", TangoImages)]
		[InlineData("3906-card.jpg", "3906-photo.jpg", TangoImages)]
		[InlineData("3921-card.png", "3921-photo.jpg", TangoImages)]
		[InlineData("3923-card.jpg", "3923-photo.jpg", TangoImages)]
		[InlineData("3925-card.jpg", "3925-photo.jpg", TangoImages)]
		[InlineData("3941-card.jpg", "3941-photo.jpg", TangoImages)]
		[InlineData("3966-card.jpeg", "3966-photo.jpeg", TangoImages)]
		[InlineData("3968-card.jpg", "3968-photo.jpg", TangoImages)]
		[InlineData("3969-card.jpg", "3969-photo.jpg", TangoImages)]
		[InlineData("3970-card.jpg", "3970-photo.jpg", TangoImages)]
		[InlineData("3973-card.jpeg", "3973-photo.jpeg", TangoImages)]
		[InlineData("3974-card.jpg", "3974-photo.jpg", TangoImages)]
		[InlineData("3977-card.jpg", "3977-photo.jpg", TangoImages)]
		[InlineData("3978-card.jpg", "3978-photo.jpg", TangoImages)]
		[InlineData("3980-card.jpg", "3980-photo.jpg", TangoImages)]
		[InlineData("110857100428-card.jpeg", "110857100428-photo.jpeg", TangoImages)]
		[InlineData("set1-car.png", "set1-photo.jpg", TangoImages)]
		[InlineData("set2-card.jpg", "set2-photo.gif", TangoImages)]
		[InlineData("set2-photo.jpg", "set2-photo.png", TangoImages)]
		public async Task TestFaceMatching(string idCardFilename, string faceFilename, string imagePath)
		{
			var wrapper = this.CreateServiceWrapper();
			idCardFilename = Path.Combine(imagePath, idCardFilename);
			faceFilename = Path.Combine(imagePath, faceFilename);
			Assert.True(File.Exists(idCardFilename));
			Assert.True(File.Exists(faceFilename));

			var idCardImage = Image.Load(idCardFilename);
			var faceImage = Image.Load(faceFilename);

			var results = await wrapper.Execute(idCardImage.Data, idCardImage.Type, faceImage.Data, faceImage.Type);
			this.Log.Write(results);

			Assert.True(results.ContainsKey("TEST_FACE_RECOGNITION_VALUE"));
			Assert.True(results.ContainsKey("TEST_FACE_RECOGNITION_RATIO"));

			Console.WriteLine(results["TEST_FACE_RECOGNITION_RATIO"]);

		}

		private async Task<string> GetRatio(string imagePath, string idCardFilename, string faceFilename, int passNumber)
		{
			var wrapper = this.CreateServiceWrapper();
			idCardFilename = Path.Combine(imagePath, idCardFilename);
			faceFilename = Path.Combine(imagePath, faceFilename);
			Assert.True(File.Exists(idCardFilename));
			Assert.True(File.Exists(faceFilename));

			var idCardImage = Image.Load(idCardFilename);
			var faceImage = Image.Load(faceFilename);

			var results = await wrapper.Execute(idCardImage.Data, idCardImage.Type, faceImage.Data, faceImage.Type);

			this.Log.Write(results);

			var key = "TEST_FACE_RECOGNITION_RATIO";
			if (results.ContainsKey(key))
			{
				return results[key].Value;
			}
			else
			{
				return "XXXXXXX";
			}



		}

		[Theory]
		[InlineData("3780-card.jpg", "3780-photo.jpg", TangoImages)]
		public async Task TestFaceMatchingConsistency(string idCardFilename, string faceFilename, string imagePath)
		{
			const int passNumber = 10;
			var ratios = new List<string>();

			for (var i = 0; i < passNumber; i++)
			{
				var ratio = await this.GetRatio(imagePath, idCardFilename, faceFilename, i);
				ratios.Add(ratio);
			}

			for (var i = 1; i < passNumber; i++)
			{
				Assert.Equal(ratios[i - 1], ratios[i]);
			}


		}
		private IDCloudServiceWrapper CreateServiceWrapper()
		{
			var configuration = new
			{
				Reference = "IdCar",
				Company = "01980",
				User = "Telindus_LM",
				Password = "Telindus2017",
				Url = "https://demoidcloud.icarvision.com:50061/iCarSAAS.WsPublic/WsDocument.asmx"
			};
			return new IDCloudServiceWrapper(configuration.Url, configuration.Company, configuration.User, configuration.Password);
		}

	}
}
