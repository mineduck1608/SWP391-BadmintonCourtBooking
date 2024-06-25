using System.Diagnostics;

namespace BadmintonCourtAPI.Utils
{
	public class Environment
	{
		private static IConfiguration _configuration;
		public static int StartHour { get; private set; }
		public static int EndHour { get; private set; }
		static Environment()
		{
			if (_configuration == null)
			{
				var configBuilder = new ConfigurationBuilder();
				Debug.WriteLine(Directory.GetCurrentDirectory());
				configBuilder.SetBasePath(Directory.GetCurrentDirectory());
				configBuilder.AddJsonFile(@"functionSettings.json");
				_configuration = configBuilder.Build();
			}
			StartHour = int.Parse(_configuration["StartHour"]);
			EndHour = int.Parse(_configuration["EndHour"]);
		}
	}
}
