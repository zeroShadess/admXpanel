using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdminPanel.Models;
using AdminPanel.ViewModels;
using AdminPanel.Services;
using AdminPanel.Data;

namespace AdminPanel.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(
            IProjectService projectService, 
            ApplicationDbContext context, 
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _projectService = projectService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, ProjectStatus? statusFilter, int? techFilter)
        {
            var userId = _userManager.GetUserId(User);
            var projects = await _projectService.GetAllProjectsAsync(userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.SiteName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (statusFilter.HasValue)
            {
                projects = projects.Where(p => p.Status == statusFilter.Value).ToList();
            }

            if (techFilter.HasValue)
            {
                projects = projects.Where(p => p.ProjectTechnologies.Any(pt => pt.TechnologyId == techFilter.Value)).ToList();
            }

            ViewBag.Technologies = await _context.Technologies.ToListAsync();
            return View(projects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _projectService.GetProjectByIdAsync(id, userId);
            if (project == null) return NotFound();

            return View(project);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ProjectCreateViewModel
            {
                AvailableTechnologies = await _context.Technologies.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageUrl = null;
                if (model.ImageFile != null)
                {
                    imageUrl = await UploadImage(model.ImageFile);
                }

                var userId = _userManager.GetUserId(User);
                await _projectService.AddProjectAsync(model, imageUrl, userId);
                TempData["SuccessMessage"] = "Proje başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            
            model.AvailableTechnologies = await _context.Technologies.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _projectService.GetProjectByIdAsync(id, userId);
            if (project == null) return NotFound();

            var model = new ProjectEditViewModel
            {
                Id = project.Id,
                SiteName = project.SiteName,
                SiteUrl = project.SiteUrl,
                Description = project.Description,
                ExistingImageUrl = project.ImageUrl,
                Status = project.Status,
                DailyVisitors = project.DailyVisitors,
                TotalVisitors = project.TotalVisitors,
                SelectedTechnologyIds = project.ProjectTechnologies.Select(pt => pt.TechnologyId).ToList(),
                AvailableTechnologies = await _context.Technologies.ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                string imageUrl = model.ExistingImageUrl;
                if (model.ImageFile != null)
                {
                    imageUrl = await UploadImage(model.ImageFile);
                }

                var userId = _userManager.GetUserId(User);
                await _projectService.UpdateProjectAsync(model, imageUrl, userId);
                TempData["SuccessMessage"] = "Proje başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            
            model.AvailableTechnologies = await _context.Technologies.ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _projectService.DeleteProjectAsync(id, userId);
            TempData["SuccessMessage"] = "Proje başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadImage(Microsoft.AspNetCore.Http.IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "projects");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            
            return "/images/projects/" + uniqueFileName;
        }
    }
}
