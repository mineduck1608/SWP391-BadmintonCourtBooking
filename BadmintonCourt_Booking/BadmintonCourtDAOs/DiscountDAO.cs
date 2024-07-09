using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
	public class DiscountDAO
	{
		private readonly BadmintonCourtContext _dbContext = null;

		public DiscountDAO()
		{
			if (_dbContext == null)
			{
				_dbContext = new BadmintonCourtContext();
			}
		}

		public DiscountDAO(BadmintonCourtContext context)
		{
			_dbContext = context;
		}

		public List<Discount> GetAllDiscounts() => _dbContext.Discounts.ToList();

		public Discount GetDiscountById(string id) => _dbContext.Discounts.FirstOrDefault(x => x.DiscountId == id);

		public void UpdateDiscount(Discount newDiscount, string id)
		{
			Discount tmp = GetDiscountById(id);
			if (tmp != null)
			{
				tmp.Proportion = newDiscount.Proportion;
				tmp.Amount = newDiscount.Amount;
				_dbContext.Discounts.Update(tmp);
				_dbContext.SaveChanges();
			}
		}

		public void AddDiscount(Discount discount)
		{
			_dbContext.Discounts.Add(discount);
			_dbContext.SaveChanges();
		}

		public void DeleteDiscount(string id)
		{
			Discount discount = GetDiscountById(id);
			if (discount != null)
			{
				discount.IsDelete = true;
				UpdateDiscount(discount, id);
			}
		}

	}
}
