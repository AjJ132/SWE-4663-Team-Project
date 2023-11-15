
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;

namespace TeamProject.Controllers
{





    //DB Controller
    // *DB Controller is used to interact with the database and hold the methods to do so
    public class DatabaseController
    {

        //DB Context
        private readonly AppDbContext _context;

        //DbContext will be passed via dependency injection <---- Program.cs
        public DatabaseController(AppDbContext context)
        {
            _context = context;
        }

        //Methods for Project
        //Get all projects
        public async Task<List<Project>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        //Get project by id
        public async Task<Project> GetProject(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        //Add project
        public async Task<Project> AddProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        //Update project
        public async Task<Project> UpdateProject(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }

        //Delete project
        public async Task<Project> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return project;
        }

        //Methods for ProjectRequirements
        //Get project requirements by project id
        public async Task<List<ProjectRequirement>> GetProjectRequirementsByProjectID(int projectId)
        {
            return await _context.ProjectRequirements.Where(x => x.ProjectId == projectId).ToListAsync();
        }

        //Get project requirement by id
        public async Task<ProjectRequirement> GetProjectRequirement(int id)
        {
            return await _context.ProjectRequirements.FindAsync(id);
        }

        //Add project requirement
        public async Task<ProjectRequirement> AddProjectRequirement(ProjectRequirement projectRequirement)
        {
            _context.ProjectRequirements.Add(projectRequirement);
            await _context.SaveChangesAsync();
            return projectRequirement;
        }

        //Update project requirement
        public async Task<ProjectRequirement> UpdateProjectRequirement(ProjectRequirement projectRequirement)
        {
            _context.Entry(projectRequirement).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return projectRequirement;
        }

        //Delete project requirement
        public async Task<ProjectRequirement> DeleteProjectRequirement(int id)
        {
            var projectRequirement = await _context.ProjectRequirements.FindAsync(id);
            _context.ProjectRequirements.Remove(projectRequirement);
            await _context.SaveChangesAsync();
            return projectRequirement;
        }

        //Methods for ProjectTeamMembers
        //Get project team members by project id
        public async Task<List<ProjectTeamMember>> GetProjectTeamMembers(int projectId)
        {
            return await _context.ProjectTeamMembers.Where(x => x.ProjectID == projectId).ToListAsync();
        }

        //Get project team member by id
        public async Task<ProjectTeamMember> GetProjectTeamMember(int id)
        {
            return await _context.ProjectTeamMembers.FindAsync(id);
        }

        //Add project team member
        public async Task<ProjectTeamMember> AddProjectTeamMember(ProjectTeamMember projectTeamMember)
        {
            _context.ProjectTeamMembers.Add(projectTeamMember);
            await _context.SaveChangesAsync();
            return projectTeamMember;
        }

        //Update project team member
        public async Task<ProjectTeamMember> UpdateProjectTeamMember(ProjectTeamMember projectTeamMember)
        {
            _context.Entry(projectTeamMember).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return projectTeamMember;
        }

        //Delete project team member
        public async Task<ProjectTeamMember> DeleteProjectTeamMember(int id)
        {
            var projectTeamMember = await _context.ProjectTeamMembers.FindAsync(id);
            _context.ProjectTeamMembers.Remove(projectTeamMember);
            await _context.SaveChangesAsync();
            return projectTeamMember;
        }

        //Methods for risks
        //Get risks by project id
        public async Task<List<Risk>> GetRisksByProjectID(int projectId)
        {
            return await _context.Risks.Where(x => x.ProjectID == projectId).ToListAsync();
        }

        //Get risk by id
        public async Task<Risk> GetRisk(int id)
        {
            return await _context.Risks.FindAsync(id);
        }

        //Add risk
        public async Task<Risk> AddRisk(Risk risk)
        {
            _context.Risks.Add(risk);
            await _context.SaveChangesAsync();
            return risk;
        }

        //Update risk
        public async Task<Risk> UpdateRisk(Risk risk)
        {
            _context.Entry(risk).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return risk;
        }

        //Delete risk
        public async Task<Risk> DeleteRisk(int id)
        {
            var risk = await _context.Risks.FindAsync(id);
            _context.Risks.Remove(risk);
            await _context.SaveChangesAsync();
            return risk;
        }

        //methods for man hours
        //get man hours by project ID
        public async Task<List<LoggedManHours>> GetLoggedManHoursByProjectID(int projectId)
        {
            return await _context.LoggedManHours.Where(lmh => lmh.ProjectID == projectId).ToListAsync();
        }

        //Add logged man hours
        public async Task<LoggedManHours> AddLoggedManHours(LoggedManHours loggedManHours)
        {
            _context.LoggedManHours.Add(loggedManHours);
            await _context.SaveChangesAsync();
            return loggedManHours;
        }

        //Methods for members
        //Get all members
        public async Task<List<TeamMember>> GetTeamMembers()
        {
            return await _context.TeamMembers.ToListAsync();
        }

        //Get member by id
        public async Task<TeamMember> GetTeamMember(int id)
        {
            return await _context.TeamMembers.FindAsync(id);
        }

        //Add member
        public async Task<TeamMember> AddTeamMember(TeamMember teamMember)
        {
            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();
            return teamMember;
        }




    }

    //DB Context
    // *DB Context is used to establish a connection to the database and hold the data
    public class AppDbContext : DbContext
    {
        //DbSets
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectRequirement> ProjectRequirements { get; set; }
        public DbSet<ProjectTeamMember> ProjectTeamMembers { get; set; }
        public DbSet<LoggedManHours> LoggedManHours { get; set; }
        public DbSet<Risk> Risks { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //DB Connection string
            optionsBuilder.UseSqlite("Data Source=Database/TeamProjDB.db3");


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .ToTable("Projects");

            modelBuilder.Entity<ProjectRequirement>()
                .ToTable("ProjectRequirements");

            modelBuilder.Entity<ProjectTeamMember>()
                .ToTable("ProjectTeamMembers");

            modelBuilder.Entity<LoggedManHours>()
                .ToTable("LoggedManHours");

            modelBuilder.Entity<Risk>()
                .ToTable("Risks");

            modelBuilder.Entity<TeamMember>()
                .ToTable("TeamMembers");

        }
    }




}