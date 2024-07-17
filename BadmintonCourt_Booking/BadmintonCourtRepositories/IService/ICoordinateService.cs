using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
	public interface ICoordinateService
	{
		public Dictionary<string, double> ExtractCoordinates(string url);
		public double CaculateDistance(double orgLatitude, double orgLongitude, double desLatitude, double desLongtitude);
	}
}
