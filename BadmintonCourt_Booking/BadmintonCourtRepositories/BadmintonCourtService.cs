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

        public readonly BookingService bookingService = null;

        public readonly CourtService courtService = null;

        public readonly CourtBranchService courtBranchService = null;

        public readonly FeedbackService feedbackService = null;

        public readonly PaymentService paymentService = null;

        public readonly RoleService roleService = null;

        public readonly SlotService slotService = null;

        public readonly UserService userService = null;

        public readonly UserDetailService userDetailService = null;

        public readonly VnPayService VnPayService = null;

     

        public BadmintonCourtService(IConfiguration config)
        {
            if (bookingService == null)
            {
                bookingService = new BookingService();
            }
            if (courtService == null)
            {
                courtService = new CourtService();
            }
            if (courtBranchService == null)
            {
                courtBranchService= new CourtBranchService();
            }
            if (feedbackService == null)
            {
                feedbackService = new FeedbackService();
            }
            if (paymentService == null)
            {
                paymentService = new PaymentService();
            }
            if (roleService == null)
            {
                roleService = new RoleService();
            }
            if (slotService == null)
            {
                slotService = new SlotService();
            }
            if (userService == null)
            {
                userService = new UserService();
            }
            if (userDetailService == null)
            {
                userDetailService = new UserDetailService();
            }
            if (VnPayService == null)
                VnPayService = new VnPayService(config);
        }

    }
}
