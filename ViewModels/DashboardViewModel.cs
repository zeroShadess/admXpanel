using System.Collections.Generic;
using AdminPanel.Models;

namespace AdminPanel.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int DevelopingProjects { get; set; }
        public int MaintenanceProjects { get; set; }
        public int DailyTotalVisitors { get; set; }
        public int TotalVisitors { get; set; }

        public List<Project> RecentlyUpdated { get; set; } = new List<Project>();
        public List<Project> RecentlyAdded { get; set; } = new List<Project>();
        public Dictionary<string, int> StatusDistribution { get; set; } = new Dictionary<string, int>();
    }
}
