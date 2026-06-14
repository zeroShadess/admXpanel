using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.ViewModels;

namespace AdminPanel.Services
{
    public class ProjectManager : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllProjectsAsync(string userId)
        {
            return await _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int id, string userId)
        {
            return await _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProjectAsync(ProjectCreateViewModel model, string imageUrl, string userId)
        {
            var project = new Project
            {
                SiteName = model.SiteName,
                SiteUrl = model.SiteUrl,
                Description = model.Description,
                ImageUrl = imageUrl,
                Status = model.Status,
                DailyVisitors = model.DailyVisitors,
                TotalVisitors = model.TotalVisitors,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                UserId = userId,
                ProjectTechnologies = model.SelectedTechnologyIds?.Select(tId => new ProjectTechnology
                {
                    TechnologyId = tId
                }).ToList() ?? new List<ProjectTechnology>()
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProjectAsync(ProjectEditViewModel model, string imageUrl, string userId)
        {
            var project = await _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.ProjectTechnologies)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (project != null)
            {
                project.SiteName = model.SiteName;
                project.SiteUrl = model.SiteUrl;
                project.Description = model.Description;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    project.ImageUrl = imageUrl;
                }
                project.Status = model.Status;
                project.DailyVisitors = model.DailyVisitors;
                project.TotalVisitors = model.TotalVisitors;
                project.UpdatedDate = DateTime.Now;

                // Update technologies
                project.ProjectTechnologies.Clear();
                if (model.SelectedTechnologyIds != null)
                {
                    foreach (var tId in model.SelectedTechnologyIds)
                    {
                        project.ProjectTechnologies.Add(new ProjectTechnology
                        {
                            ProjectId = project.Id,
                            TechnologyId = tId
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProjectAsync(int id, string userId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var projects = await _context.Projects.Where(p => p.UserId == userId).ToListAsync();
            
            return new DashboardViewModel
            {
                TotalProjects = projects.Count,
                ActiveProjects = projects.Count(p => p.Status == ProjectStatus.Active),
                DevelopingProjects = projects.Count(p => p.Status == ProjectStatus.Developing),
                MaintenanceProjects = projects.Count(p => p.Status == ProjectStatus.Maintenance),
                DailyTotalVisitors = projects.Sum(p => p.DailyVisitors),
                TotalVisitors = projects.Sum(p => p.TotalVisitors),
                RecentlyUpdated = projects.OrderByDescending(p => p.UpdatedDate).Take(5).ToList(),
                RecentlyAdded = projects.OrderByDescending(p => p.CreatedDate).Take(5).ToList(),
                StatusDistribution = projects.GroupBy(p => p.Status).ToDictionary(g => g.Key.ToString(), g => g.Count())
            };
        }
    }
}
