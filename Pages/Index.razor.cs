using Microsoft.AspNetCore.Components;
using System.Linq;
using TeamProject.Data;
using TeamProject.Controllers;
using System.Diagnostics;

namespace TeamProject.Pages
{
    public class IndexBase : ComponentBase
    {
        //DB Controller
        [Inject]
        private DatabaseController _dbController { get; set; }

        //Projects in the database
        public List<Project> Projects { get; set; }
        protected override async void OnInitialized()
        {
            var loadSuccessful = await StartupSequence();

            if (loadSuccessful)
                Console.WriteLine("IndexBase Initialized Successfully");
            else
                Console.WriteLine("IndexBase Failed to Initialize. Read Console for errors.");

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
                        var newProj = new Project(1);

                        //SAVE TO DATABASE
                        await _dbController.AddProject(newProj);

                        //RELOAD PROJECTS
                        projects = await _dbController.GetProjects();

                        //LOAD PROJECTS INTO PROJECTS
                        Projects = projects;
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

                        //Members

                        //risks

                        //manhours
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
    }
}