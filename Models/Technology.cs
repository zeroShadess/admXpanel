using System.Collections.Generic;

namespace AdminPanel.Models
{
    public class Technology
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<ProjectTechnology> ProjectTechnologies { get; set; }
    }
}
