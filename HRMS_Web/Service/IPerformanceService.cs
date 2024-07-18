namespace HRMS_Web.Services
{
    using HRMS_Web.Models;
  

    public interface IPerformanceService
    {
        PerformanceDashboardViewModel GetDashboardData(string userName);
    }
}
