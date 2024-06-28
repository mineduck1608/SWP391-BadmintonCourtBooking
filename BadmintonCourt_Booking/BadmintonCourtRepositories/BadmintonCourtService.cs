using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices.IService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
    public class BadmintonCourtService
    {

        public readonly BookingService BookingService = null;

        public readonly CourtService CourtService = null;

        public readonly CourtBranchService BranchService = null;

		public readonly DiscountService DiscountService = null;

		public readonly FeedbackService FeedbackService = null;

        public readonly PaymentService PaymentService = null;

        public readonly RoleService RoleService = null;

        public readonly SlotService SlotService = null;

        public readonly UserService UserService = null;

        public readonly UserDetailService UserDetailService = null;

        public readonly VnPayService VnPayService = null;

        public readonly MoMoService MomoService = null;

        public readonly MailService MailService = null;
     

        public BadmintonCourtService(IConfiguration config)
        {
            if (BookingService == null)
            {
                BookingService = new BookingService();
            }
            if (CourtService == null)
            {
                CourtService = new CourtService();
            }
            if (BranchService == null)
            {
                BranchService= new CourtBranchService();
            }
            if (FeedbackService == null)
            {
                FeedbackService = new FeedbackService();
            }
            if (PaymentService == null)
            {
                PaymentService = new PaymentService();
            }
            if (RoleService == null)
            {
                RoleService = new RoleService();
            }
            if (SlotService == null)
            {
                SlotService = new SlotService();
            }
            if (UserService == null)
            {
                UserService = new UserService();
            }
            if (UserDetailService == null)
            {
                UserDetailService = new UserDetailService();
            }
            if (VnPayService == null)
                VnPayService = new VnPayService(config);
            if (DiscountService == null)
                DiscountService = new DiscountService();
            if (MomoService == null)
                MomoService = new MoMoService();
            if (MailService == null)
                MailService = new MailService();
        }

    }
}
