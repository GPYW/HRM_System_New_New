using HRMS_Web.Models;
using HRMS_Web.Services;

namespace HRMS_Web.Service
{

    public class PerformanceService : IPerformanceService
    {
        public PerformanceDashboardViewModel GetDashboardData(string userName)
        {
            // Fetch data from database or other sources
            return new PerformanceDashboardViewModel
            {
                EmployeeName = "John Doe",
                JobTitle = "Software Engineer",
                Department = "Development",
                PhotoUrl = "/images/johndoe.jpg",
                PerformanceScore = 4.5m,
                Goals = new List<Goal>
                {
                    new Goal { Description = "Complete Project A", Progress = 80 },
                    new Goal { Description = "Improve Code Quality", Progress = 60 }
                },
                Reviews = new List<PerformanceReview>
                {
                    new PerformanceReview { ReviewPeriod = "Q1 2024", Rating = "Excellent", Comments = "Great work!" }
                },
                RecentFeedback = new List<Feedback>
                {
                    new Feedback { From = "Jane Smith", Message = "Outstanding collaboration!" }
                },
                Trainings = new List<Training>
                {
                    new Training { CourseName = "ASP.NET Core", Status = "Completed" }
                },
                Achievements = new List<Achievement>
                {
                    new Achievement { Title = "Employee of the Month", Description = "For outstanding performance in June 2024" }
                }
            };
        }
    }
}

