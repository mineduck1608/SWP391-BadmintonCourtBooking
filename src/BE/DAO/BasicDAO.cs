using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
	internal class BasicDAO
	{
		private readonly BadmintonCourtContext context;
		public BasicDAO()
		{
			context = context == null ? new BadmintonCourtContext() : context;
		}

	}
}
