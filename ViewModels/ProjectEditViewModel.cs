using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using AdminPanel.Models;

namespace AdminPanel.ViewModels
{
    public class ProjectEditViewModel
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public string Description { get; set; }
        public string? ExistingImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public ProjectStatus Status { get; set; }
        public int DailyVisitors { get; set; }
        public int TotalVisitors { get; set; }
        
        public List<int> SelectedTechnologyIds { get; set; } = new List<int>();
        public List<Technology> AvailableTechnologies { get; set; } = new List<Technology>();
    }
}
