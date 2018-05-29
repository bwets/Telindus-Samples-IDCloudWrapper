using System.IO;
using System.Threading.Tasks;

namespace IDCloudWrapper {
	public static class Template
	{
		private static async Task<string> GetTemplate(string name)
		{
			var assembly = typeof(IDCloudServiceWrapper).Assembly;
			var ns       = typeof(IDCloudServiceWrapper).Namespace + ".Resources";
			using (var stream = assembly.GetManifestResourceStream(ns + "." + name + ".xml"))
			{
				using (var reader = new StreamReader(stream))
				{
					return await reader.ReadToEndAsync();
				}
			}

		}
		public static async Task<string> For1Image()
		{
			return await GetTemplate("IDCloud1Image");
		}

		public static async Task<string> For2Images()
		{
			return await GetTemplate("IDCloud2Images");
		}
	}
}