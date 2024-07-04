using System;
using System.Collections.Generic;
using HRMS_Web.Models;

namespace HRMS_Web.ViewModels
{
    public class LeaveHistoryViewModel
    {
        public List<LeaveRequestModel> PendingRequests { get; set; }
        public List<LeaveRequestModel> LeaveHistory { get; set; }
    }
}
