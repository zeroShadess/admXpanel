using System;
using System.Collections.Generic;

namespace AdminPanel.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ProjectStatus Status { get; set; }
        public int DailyVisitors { get; set; }
        public int TotalVisitors { get; set; }
        
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<ProjectTechnology> ProjectTechnologies { get; set; }
    }
}
