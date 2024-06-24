using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using BadmintonCourtServices.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{

	public class DiscountService : IDiscountService
	{
		private readonly DiscountDAO _discountDAO = null;

		public DiscountService()
		{
			if (_discountDAO == null)
				_discountDAO = new DiscountDAO();
		}
		public void AddDiscount(Discount discount) => _discountDAO.AddDiscount(discount);

		public void DeleteRole(string id) => _discountDAO.DeleteDiscount(id);

		public List<Discount> GetAllDiscounts() => _discountDAO.GetAllDiscounts();
		public Discount GetDiscountById(string id) => _discountDAO.GetDiscountById(id);
		public void UpdateDiscount(Discount newDiscount, string id) => _discountDAO.UpdateDiscount(newDiscount, id);
	}
}
