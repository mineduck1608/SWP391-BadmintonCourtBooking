using BadmintonCourtServices.IService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
	public class CoordinateService : ICoordinateService
	{

		private double ToRadians(double x) => x * Math.PI / 180;
		public double CaculateDistance(double orgLatitude, double orgLongitude, double desLatitude, double desLongitude)
		{
			const double r = 6371; // Radius of Earth in kilometers
			double latDistance = ToRadians(orgLatitude - desLatitude);
			double lonDistance = ToRadians(orgLongitude - desLongitude);
			double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
					   Math.Cos(ToRadians(orgLatitude)) * Math.Cos(ToRadians(desLatitude)) *
					   Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return r * c * 1000; // meters
		}

		public Dictionary<string, double> ExtractCoordinates(string url)
		{
			// Find indices of latitude and longitude values
			int latIndex = url.IndexOf("!3d") + 3; // Start index of latitude
			int lngIndex = url.IndexOf("!2d") + 3; // Start index of longitude

			// Extract latitude and longitude values
			string rawLatitude = url.Substring(latIndex, url.IndexOf('!', latIndex) - latIndex);
			string rawLongitude = url.Substring(lngIndex, url.IndexOf('!', lngIndex) - lngIndex);
			Dictionary<string, double> result = new Dictionary<string, double>();
			try
			{
				double latitude = double.Parse(rawLatitude);
				result.Add("Latitude", latitude);
				double longitude = double.Parse(rawLongitude);
				result.Add("Longtitude", longitude);
				return result;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				return null;
			}
		}
	}
}
