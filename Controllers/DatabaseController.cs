
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