using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
	public interface IMailService
	{
		public void SendMail(string des, string content, string subject);

	}
}
