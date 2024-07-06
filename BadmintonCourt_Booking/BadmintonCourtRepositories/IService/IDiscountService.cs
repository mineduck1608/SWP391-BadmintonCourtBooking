using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
	public interface IDiscountService
	{
		public List<Discount> GetAllDiscounts();

		public Discount GetDiscountById(string id);

		public void UpdateDiscount(Discount newDiscount, string id);

		public void AddDiscount(Discount discount);

		public void DeleteDiscount(string id);
	}
}
