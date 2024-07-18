using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class PerformanceDashboardViewModel
    {
        public string EmployeeName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string PhotoUrl { get; set; }
        public decimal PerformanceScore { get; set; }
        public List<Goal> Goals { get; set; }
        public List<PerformanceReview> Reviews { get; set; }
        public List<Feedback> RecentFeedback { get; set; }
        public List<Training> Trainings { get; set; }
        public List<Achievement> Achievements { get; set; }
    }

    public class Goal
    {
        public string Description { get; set; }
        public decimal Progress { get; set; }
    }

    public class PerformanceReview
    {
        public string ReviewPeriod { get; set; }
        public string Rating { get; set; }
        public string Comments { get; set; }
    }
    public class Feedback
    {
        public string From { get; set; }
        public string Message { get; set; }
    }

    public class Training
    {
        public string CourseName { get; set; }
        public string Status { get; set; }
    }

    public class Achievement
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }


}
