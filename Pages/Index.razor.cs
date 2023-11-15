using Microsoft.AspNetCore.Components;
using System.Linq;
using TeamProject.Data;
using TeamProject.Controllers;
using System.Diagnostics;
using Newtonsoft.Json;


namespace TeamProject.Pages
{
    public class IndexBase : ComponentBase
    {
        //DB Controller
        [Inject]
        private DatabaseController _dbController { get; set; }

        //Projects in the database
        public List<Project> Projects { get; set; }

        //Bool to check if info is loaded
        public bool IsLoaded { get; set; } = false;
        public bool ErrorOccured { get; set; } = false;
        protected override async void OnInitialized()
        {
            var loadSuccessful = await StartupSequence();

            if (loadSuccessful)
            {


                Console.WriteLine("IndexBase Initialized Successfully");

                //DEV CODE
                //SAVE PROJECTS TO JSON FILE
                var json = JsonConvert.SerializeObject(Projects);
                System.IO.File.WriteAllText(@"./wwwroot/test.json", json);

            }
            else
            {
                Console.WriteLine("IndexBase Failed to Initialize. Read Console for errors.");
                ErrorOccured = true;
            }


            IsLoaded = true;
            base.OnInitialized();
        }

        private async Task<bool> StartupSequence()
        {
            try
            {
                //load projects from database
                var projects = await _dbController.GetProjects();
                if (projects == null)
                {
                    Console.WriteLine("Failed to load projects from database.");
                    return false;
                }
                else
                {
                    //TEMP DEVELOPMENT CODE
                    //IF PROJECTS IS = 0, THEN INSERT SOME TEST DATA
                    if (projects.Count == 0)
                    {
                        var success = await CreateTestData();

                        if (!success)
                        {
                            Console.WriteLine("Failed to create test data.");
                            return false;
                        }
                        else
                        {
                            //load projects into Projects
                            Projects = projects;
                        }
                    }
                    else
                    {
                        //load projects into Projects
                        Projects = projects;
                    }

                    Console.WriteLine("Projects loaded from database.");
                    projects = null; //discard to clear memory

                    //for each project, loop over and get relationship objects. (risks, requirements, etc)
                    foreach (var proj in this.Projects)
                    {
                        //Requirements
                        var requirements = await _dbController.GetProjectRequirementsByProjectID(proj.Id);

                        //null check
                        if (requirements == null)
                        {
                            Console.WriteLine("Failed to load requirements for project: " + proj.Name);
                            return false;
                        }
                        else
                        {
                            //load requirements into project
                            proj.Requirements = requirements;
                        }

                        //Members
                        var members = await _dbController.GetProjectTeamMembers(proj.Id);

                        //null check
                        if (members == null)
                        {
                            Console.WriteLine("Failed to load members for project: " + proj.Name);
                            return false;
                        }
                        else
                        {
                            //load members into project
                            proj.TeamMembers = members;
                        }

                        //risks
                        var risks = await _dbController.GetRisksByProjectID(proj.Id);

                        //null check
                        if (risks == null)
                        {
                            Console.WriteLine("Failed to load risks for project: " + proj.Name);
                            return false;
                        }
                        else
                        {
                            //load risks into project
                            proj.Risks = risks;
                        }

                        //manhours
                        var manhours = await _dbController.GetLoggedManHoursByProjectID(proj.Id);

                        //null check
                        if (manhours == null)
                        {
                            Console.WriteLine("Failed to load manhours for project: " + proj.Name);
                            return false;
                        }
                        else
                        {
                            //load manhours into project
                            proj.LoggedManHours = manhours;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an issue in the Startup Sequence.");
                Debug.WriteLine("There was an issue in the Startup Sequence.");
                Debug.WriteLine(e.Message);
                return false;
            }


            return true;
        }

        private async Task<bool> CreateTestData()
        {
            try
            {
                //Create new project
                var project = new Project(1);

                //Create new requirements
                var requirement1 = new ProjectRequirement(1);
                var requirement2 = new ProjectRequirement(2);
                var requirement3 = new ProjectRequirement(3);

                //Create new risks
                var risk1 = new Risk(1);
                var risk2 = new Risk(2);
                var risk3 = new Risk(3);

                //Create new manhours
                var manhours1 = new LoggedManHours(1, 1);
                var manhours2 = new LoggedManHours(2, 2);
                var manhours3 = new LoggedManHours(3, 3);

                //Create new members and assign them
                var member1 = new ProjectTeamMember(1);
                var member2 = new ProjectTeamMember(2);
                var member3 = new ProjectTeamMember(3);

                //set one member as project owner
                member1.PermissionLevel = 3;

                //Save all to database
                project = await _dbController.AddProject(project);
                requirement1 = await _dbController.AddProjectRequirement(requirement1);
                requirement2 = await _dbController.AddProjectRequirement(requirement2);
                requirement3 = await _dbController.AddProjectRequirement(requirement3);

                risk1 = await _dbController.AddRisk(risk1);
                risk2 = await _dbController.AddRisk(risk2);
                risk3 = await _dbController.AddRisk(risk3);

                manhours1 = await _dbController.AddLoggedManHours(manhours1);
                manhours2 = await _dbController.AddLoggedManHours(manhours2);
                manhours3 = await _dbController.AddLoggedManHours(manhours3);

                member1 = await _dbController.AddProjectTeamMember(member1);
                member2 = await _dbController.AddProjectTeamMember(member2);
                member3 = await _dbController.AddProjectTeamMember(member3);

                project.ProjectOwnerID = member1.TeamMemberID;

                //update project
                //project = await _dbController.UpdateProject(project);
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an issue creating test data.");
                Debug.WriteLine("There was an issue creating test data.");
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }
    }
}