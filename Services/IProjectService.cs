using System.Collections.Generic;
using System.Threading.Tasks;
using AdminPanel.Models;
using AdminPanel.ViewModels;

namespace AdminPanel.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllProjectsAsync(string userId);
        Task<Project> GetProjectByIdAsync(int id, string userId);
        Task AddProjectAsync(ProjectCreateViewModel model, string imageUrl, string userId);
        Task UpdateProjectAsync(ProjectEditViewModel model, string imageUrl, string userId);
        Task DeleteProjectAsync(int id, string userId);
        Task<DashboardViewModel> GetDashboardDataAsync(string userId);
    }
}
