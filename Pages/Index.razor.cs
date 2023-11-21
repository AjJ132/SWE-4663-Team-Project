using Microsoft.AspNetCore.Components;
using System.Linq;
using TeamProject.Data;
using TeamProject.Controllers;
using System.Diagnostics;
using Newtonsoft.Json;
using Radzen.Blazor;
using Radzen;
using System.Security.Cryptography;
using SWE_4663_Team_Project.Pages;
using Microsoft.EntityFrameworkCore.Metadata;


namespace TeamProject.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        private NotificationService _notificationService { get; set; }

        //DB Controller
        [Inject]
        private DatabaseController _dbController { get; set; }

        [Inject]
        private DialogService _dialogService { get; set; }

        //Projects in the database
        public List<Project> Projects { get; set; }

        //bool to control dialog box for selecting user
        public bool ShowUserDialogBox { get; set; } = false;

        public RadzenDataGrid<Risk> RisksGrid { get; set; }
        public Risk RiskToUpdate { get; set; }
        public Risk RiskToInsert { get; set; }



        //Bool to check if info is loaded
        public bool IsLoaded { get; set; } = false;
        public bool ErrorOccured { get; set; } = false;
        protected override async void OnInitialized()
        {
            var projectsLoadSuccessful = await StartupSequence();

            if (projectsLoadSuccessful)
            {
                Console.WriteLine("Projects Retrieved from Database.");
                var ghanntChartDatesSuccessful = CalculateGhanntChartDates();

                if (ghanntChartDatesSuccessful)
                {
                    Console.WriteLine("Ghannt Chart Dates Calculated.");
                }
                else
                {
                    Console.WriteLine("Failed to Calculate Ghannt Chart Dates.");
                    ErrorOccured = true;
                }
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
                        success = await CreateTestData2();
                        success = await CreateTestData3();

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
                            //set default requirements
                            proj.Requirements = new List<ProjectRequirement>();
                        }
                        else
                        {
                            //load requirements into project
                            Console.WriteLine("Requirements Count: " + requirements.Count());
                            proj.Requirements = requirements;
                        }

                        //Members
                        var members = await _dbController.GetProjectTeamMembers(proj.Id);

                        //null check
                        if (members == null)
                        {
                            Console.WriteLine("Failed to load members for project: " + proj.Name);
                            proj.TeamMembers = new List<ProjectTeamMember>();
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
                            proj.Risks = new List<Risk>();
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
                            proj.LoggedManHours = new List<LoggedManHours>();
                        }
                        else
                        {
                            //load manhours into project
                            proj.LoggedManHours = manhours;
                        }

                        //project phase hours
                        var projectPhaseHours = await _dbController.GetProjectPhaseHoursByProjID(proj.Id);

                        //null check
                        if (projectPhaseHours == null)
                        {
                            Console.WriteLine("Failed to load project phase hours for project: " + proj.Name);
                            proj.ProjectPhaseHours = new ProjectPhaseHours();
                        }
                        else
                        {
                            //load project phase hours into project
                            proj.ProjectPhaseHours = projectPhaseHours;
                        }

                        //Set Project Stats
                        proj.SetProjectStats();
                        Console.WriteLine("Project Stats Set for: " + proj.Name);
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

        private bool CalculateGhanntChartDates()
        {
            try
            {
                //loop over each project
                foreach (var proj in this.Projects)
                {
                    proj.Tasks = new List<TaskModel>();
                    var startDate = proj.ProjectStartDate;
                    var endDate = DateTime.Today;

                    List<DateTime> dates = new List<DateTime>();

                    //Fill dates list with each day between start and end date
                    for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                    {
                        dates.Add(dt);
                        proj.ProjectDates.Add(dt);
                    }

                    //get all requirements for project
                    var requirements = proj.Requirements;

                    //Get all manhours for project
                    var manhours = proj.LoggedManHours;

                    //loop over each requirement
                    foreach (var req in requirements)
                    {
                        var task = new TaskModel
                        {
                            TaskName = req.Title,
                            Dates = new SortedList<DateTime, bool>(),
                            Status = req.Status.ToString(),
                            ReqID = req.RequirementId
                        };

                        //check man hours to see if requirment id match
                        var work = manhours.Where(x => x.RequirementID == req.RequirementId).ToList();

                        //if work isnt empty then find the dates that work took place, add to dictionary and set to true
                        if (work.Count > 0)
                        {
                            foreach (var w in work)
                            {
                                var date = w.Date;
                                task.Dates.Add(date, true);
                                task.UserID = w.TeamMemberID;
                            }
                        }

                        //loop over each date in dates list
                        foreach (var date in dates)
                        {
                            //if the date is not in the dictionary then add it and set to false
                            if (!task.Dates.ContainsKey(date))
                            {
                                task.Dates.Add(date, false);
                            }
                        }

                        proj.Tasks.Add(task);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an issue calculating the Ghannt Chart Dates.");
                Console.WriteLine(e.Message);
                Debug.WriteLine("There was an issue calculating the Ghannt Chart Dates.");

                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async void ToggleTaskDate(TaskModel task, DateTime date, int projectID)
        {

            Console.WriteLine("Toggle Task Date");
            //First present user with dialog to select name from dropbox
            //Gather available users for project and construct dictionary

            var members = this.Projects.Where(x => x.Id == projectID).FirstOrDefault().TeamMembers;
            var users = new Dictionary<int, string>();
            users = members.ToDictionary(x => x.TeamMemberID, x => x.TeamMemberName);

            var result = await _dialogService.OpenAsync<UserSelection>("", new Dictionary<string, object>() { { "Users", users } }, new DialogOptions() { Resizable = true, Draggable = true, CloseDialogOnOverlayClick = true });
            if (result != null && result is Tuple<int, int> selectedUserId)
            {
                selectedUserId = (Tuple<int, int>)result;
                Console.WriteLine("User Selected: " + users.Where(x => x.Key == selectedUserId.Item1).FirstOrDefault().Value);

                // if task is true then toggle off otherwise toggle on
                var taskModel = task.Dates.Where(x => x.Key == date).FirstOrDefault();

                if (taskModel.Value == true)
                {
                    task.Dates[date] = false;
                }
                else
                {
                    task.Dates[date] = true;
                }

                //Update project
                this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Tasks.Where(x => x.TaskName == task.TaskName).FirstOrDefault().Dates = task.Dates;

                //Update database
                var manhours = new LoggedManHours(projectID, selectedUserId.Item1, selectedUserId.Item2, date, task.ReqID, "Implementation");

                manhours = await _dbController.AddLoggedManHours(manhours);

                //refresh grid
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                Console.WriteLine("Exited");
                return;
            }





        }

        public async void ToggleRequirementStatus(int reqId, int projectID)
        {
            //First identify the requirement
            var requirement = this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Requirements.Where(x => x.RequirementId == reqId).FirstOrDefault();

            var status = requirement.Status;

            var result = await _dialogService.OpenAsync<StatusSelection>("", new Dictionary<string, object>() { { "Status", status } }, new DialogOptions() { Resizable = true, Draggable = true, CloseDialogOnOverlayClick = true });

            if (result != null && result is RequirementStatus newStatus)
            {
                newStatus = (RequirementStatus)result;
                Console.WriteLine("New Status Selected: " + newStatus.ToString());
                return;
            }
        }

        public async void CreateNewRequirement(int projectID)
        {
            var ProjectRequirement = new ProjectRequirement();
            ProjectRequirement.ProjectId = projectID;
            var result = await _dialogService.OpenAsync<AddRequirement>("Add Requirement", new Dictionary<string, object>() { { "ProjectRequirement", ProjectRequirement } }, new DialogOptions() { Width = "600px", Height = "600px" });
            if (result != null && result is ProjectRequirement newRequirement)
            {
                newRequirement = (ProjectRequirement)result;
                Console.WriteLine("New Requirement Created: " + newRequirement.Title);

                //Add to database
                newRequirement = await _dbController.AddProjectRequirement(newRequirement);

                //Add to project
                this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Requirements.Add(newRequirement);

                //refresh grid
                await InvokeAsync(StateHasChanged);
            }
        }

        public async void UpdateRequrementStatus(int projectId, int requirementID)
        {
            string status = this.Projects.Where(n => n.Id == projectId).FirstOrDefault().Requirements.Where(x => x.RequirementId == requirementID).FirstOrDefault().Status.ToString();

            var result = await _dialogService.OpenAsync<StatusSelection>("Status", new Dictionary<string, object>() { { "Status", status } }, new DialogOptions() { Resizable = true, Draggable = true, CloseDialogOnOverlayClick = true });

            if (result != null && result is RequirementStatus newStatus)
            {
                newStatus = (RequirementStatus)result;
                Console.WriteLine("New Status Selected: " + newStatus.ToString());

                //Update database
                var requirement = this.Projects.Where(n => n.Id == projectId).FirstOrDefault().Requirements.Where(x => x.RequirementId == requirementID).FirstOrDefault();
                requirement.Status = newStatus;
                requirement = await _dbController.UpdateProjectRequirement(requirement);

                //Update project
                this.Projects.Where(n => n.Id == projectId).FirstOrDefault().Requirements.Where(x => x.RequirementId == requirementID).FirstOrDefault().Status = newStatus;

                //refresh grid
                await InvokeAsync(StateHasChanged);

                //Notify User to refresh screen
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info,
                    Summary = "Refresh Required",
                    Detail = "Please refresh the page to see the updated status."
                });
            }
        }

        public async void UpdateOrAddRisk(int projectID, int riskID)
        {
            if (riskID == -1)
            {
                //add risk
                var risk = new Risk();
                risk.ProjectID = projectID;

                var result = await _dialogService.OpenAsync<AddRisk>("AddRisk", new Dictionary<string, object>() { { "Risk", risk } }, new DialogOptions() { Width = "600px", Height = "600px" });

                if (result != null && result is Risk newRisk)
                {
                    newRisk = (Risk)result;
                    Console.WriteLine("New Risk Created: " + newRisk.RiskName);
                    newRisk.RiskID = this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.LastOrDefault().RiskID + 1;

                    //Add to database
                    newRisk = await _dbController.AddRisk(newRisk);

                    //Add to project
                    this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.Add(newRisk);

                    //refresh grid
                    await InvokeAsync(StateHasChanged);

                    //Notify User to refresh screen
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Info,
                        Summary = "Refresh Required",
                        Detail = "Please refresh the page to see the updated status."
                    });
                }

            }
            else
            {
                //update risk

                var risk = this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.Where(x => x.RiskID == riskID).FirstOrDefault();

                var result = await _dialogService.OpenAsync<AddRisk>("AddRisk", new Dictionary<string, object>() { { "Risk", risk } }, new DialogOptions() { Width = "600px", Height = "600px" });
                if (result != null && result is Risk newRisk)
                {
                    newRisk = (Risk)result;
                    Console.WriteLine("New Risk Created: " + newRisk.RiskName);

                    //Update database
                    newRisk = await _dbController.UpdateRisk(newRisk);

                    //Update project
                    this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.Where(x => x.RiskID == riskID).FirstOrDefault().RiskName = newRisk.RiskName;
                    this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.Where(x => x.RiskID == riskID).FirstOrDefault().RiskDescription = newRisk.RiskDescription;
                    this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.Where(x => x.RiskID == riskID).FirstOrDefault().RiskSeverity = newRisk.RiskSeverity;
                    this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.Where(x => x.RiskID == riskID).FirstOrDefault().RiskStatus = newRisk.RiskStatus;
                    this.Projects.Where(x => x.Id == projectID).FirstOrDefault().Risks.Where(x => x.RiskID == riskID).FirstOrDefault().RiskMitigation = newRisk.RiskMitigation;

                    //refresh grid
                    await InvokeAsync(StateHasChanged);

                    //Notify User to refresh screen
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Info,
                        Summary = "Refresh Required",
                        Detail = "Please refresh the page to see the updated status."
                    });
                }
            }
        }

        public async void CreateNewTeamMember(int projectID)
        {
            var teamMember = new TeamMember();
            var result = await _dialogService.OpenAsync<CreateMembers>("Add Team Member", new Dictionary<string, object>() { { "TeamMember", teamMember } }, new DialogOptions() { Width = "600px", Height = "600px" });
            if (result != null && result is TeamMember newTeamMember)
            {
                newTeamMember = (TeamMember)result;
                Console.WriteLine("New Team Member Created: " + newTeamMember.Name);

                if (newTeamMember.MemberID == -1)
                {
                    //create new team member
                    //add to database
                    newTeamMember = await _dbController.AddTeamMember(newTeamMember);

                    //prompt to refresh page
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Info,
                        Summary = "Refresh Required",
                        Detail = "Please refresh the page to see the updated status."
                    });

                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    var ProjectTeamMember = new ProjectTeamMember(projectID, newTeamMember, 1);

                    //add to database
                    ProjectTeamMember = await _dbController.AddProjectTeamMember(ProjectTeamMember);

                    //add to project
                    this.Projects.Where(x => x.Id == projectID).FirstOrDefault().TeamMembers.Add(ProjectTeamMember);

                    //prompt to refresh page
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Info,
                        Summary = "Refresh Required",
                        Detail = "Please refresh the page to see the updated status."
                    });
                }
            }
        }

        public async void CreateNewProject()
        {
            var newProj = new Project();
            var result = await _dialogService.OpenAsync<NewProject>("Create Project", new Dictionary<string, object>() { { "Project", newProj } }, new DialogOptions() { Width = "600px", Height = "600px" });

            if (result != null && result is Project project)
            {
                project = (Project)result;
                Console.WriteLine("New Project Created: " + project.Name);

                // //add to database
                // project = await _dbController.AddProject(project);

                //add to projects
                this.Projects.Add(project);

                //prompt to refresh page
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info,
                    Summary = "Refresh Required",
                    Detail = "Please refresh the page to see the updated status."
                });

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                //Notify User project creation was cancelled
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info,
                    Summary = "Project Creation Cancelled",
                    Detail = "Project Creation Was Cancelled."
                });
            }
        }
        private async Task<bool> CreateTestData()
        {
            //
            //  CREATE TEST DATA
            //  In the even that no data is present in the SQLite database or it is deleted then this method will create some test data.
            //
            try
            {
                //Create new project
                var project = new Project(
                    name: "Class Registration System",
                    description: "A web-based class registration system for a university.",
                    projectOwnerID: 1,
                    projectOwnerName: "John Doe",
                    projectPhase: 3,
                    projectStartDate: DateTime.Now.AddDays(-10)
                );

                //Create new requirements
                // Real-Time Class Seat Availability Display
                var requirement1 = new ProjectRequirement(
                    1,
                    title: "Real-Time Class Seat Availability Display",
                    description: "Implement a feature that shows real-time availability of seats in each class during the registration process.",
                    type: RequirementType.Functional,
                    status: RequirementStatus.Implemented,
                    priority: "High",
                    effortEstimation: "15 hours",
                    notes: "Requires integration with the university's database to fetch and update seat counts in real-time."
                );

                // Automated Prerequisite Verification
                var requirement2 = new ProjectRequirement(
                    1,
                    title: "Automated Prerequisite Verification",
                    description: "Develop an automated system to check if a student meets all prerequisite requirements for a selected course.",
                    type: RequirementType.Functional,
                    status: RequirementStatus.InProgress,
                    priority: "Medium",
                    effortEstimation: "20 hours",
                    notes: "Integration with the academic records system is necessary to access student and course data."
                );

                // Mobile-Friendly Registration Interface
                var requirement3 = new ProjectRequirement(
                    1,
                    title: "Mobile-Friendly Registration Interface",
                    description: "Create a mobile-friendly version of the class registration system to enhance accessibility for students using mobile devices.",
                    type: RequirementType.NonFunctional,
                    status: RequirementStatus.Accepted,
                    priority: "Low",
                    effortEstimation: "25 hours",
                    notes: "Focus on responsive design and test across various mobile devices for compatibility."
                );


                //Create new risks
                var risk1 = new Risk(
                    projectid: 1,
                    riskid: 101,
                    riskname: "Database Overload",
                    riskdescription: "High traffic during peak registration periods might overload the database, leading to slow performance or system crashes.",
                    riskseverity: 4,
                    riskstatus: true,
                    riskmitigation: "Implement load balancing and optimize database queries to handle increased traffic."
                );

                var risk2 = new Risk(
                    projectid: 1,
                    riskid: 102,
                    riskname: "Prerequisite Check Error",
                    riskdescription: "Automated prerequisite verification might inaccurately approve or deny student registrations due to outdated or incorrect data.",
                    riskseverity: 3,
                    riskstatus: true,
                    riskmitigation: "Regularly update course data and implement a manual verification option as a backup."
                );

                var risk3 = new Risk(
                    projectid: 1,
                    riskid: 103,
                    riskname: "Mobile Compatibility",
                    riskdescription: "The mobile interface may not be compatible with all types and versions of mobile devices and browsers.",
                    riskseverity: 2,
                    riskstatus: true,
                    riskmitigation: "Conduct extensive testing on various devices and browsers; plan for ongoing updates and bug fixes."
                );


                //Create new manhours
                var manhours1 = new LoggedManHours(1, 1, 3, DateTime.Now.AddDays(-5), 1, "Design");
                var manhours2 = new LoggedManHours(1, 3, 5, DateTime.Now.AddDays(-4), 2, "Development");
                var manhours3 = new LoggedManHours(1, 1, 2, DateTime.Now.AddDays(-3), 1, "Implementation");

                var projectPhaseHours1 = new ProjectPhaseHours(1, 1, 4, 3, 2, 1);

                //Create new members and assign them
                var teamMember1 = new ProjectTeamMember(projectid: 1, teammemberid: 1, teammembername: "John Doe", permissionlevel: 3, permissionlevelname: "Project Owner");
                var teamMember2 = new ProjectTeamMember(projectid: 1, teammemberid: 2, teammembername: "Jane Doe", permissionlevel: 2, permissionlevelname: "Project Manager");
                var teamMember3 = new ProjectTeamMember(projectid: 1, teammemberid: 3, teammembername: "John Smith", permissionlevel: 1, permissionlevelname: "Team Member");

                var user1 = new TeamMember("John Doe");
                var user2 = new TeamMember("Jane Doe");
                var user3 = new TeamMember("John Smith");

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

                projectPhaseHours1 = await _dbController.AddProjectPhaseHours(projectPhaseHours1);

                teamMember1 = await _dbController.AddProjectTeamMember(teamMember1);
                teamMember2 = await _dbController.AddProjectTeamMember(teamMember2);
                teamMember3 = await _dbController.AddProjectTeamMember(teamMember3);

                user1 = await _dbController.AddTeamMember(user1);
                user2 = await _dbController.AddTeamMember(user2);
                user3 = await _dbController.AddTeamMember(user3);


                project.ProjectOwnerID = user1.MemberID;

                //update project with new owner
                project = await _dbController.UpdateProject(project);
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

        private async Task<bool> CreateTestData2()
        {
            try
            {
                // Create new project
                var project2 = new Project(
                    name: "Online Library Management System",
                    description: "A comprehensive system for managing digital and physical books in a library.",
                    projectOwnerID: 2,
                    projectOwnerName: "Alice Johnson",
                    projectPhase: 2,
                    projectStartDate: DateTime.Now.AddDays(-15)
                );

                // Create new requirements
                var requirement1 = new ProjectRequirement(
                    2,
                    title: "Digital Catalogue Integration",
                    description: "Enable integration with existing digital catalogues for seamless book management.",
                    type: RequirementType.Functional,
                    status: RequirementStatus.Proposed,
                    priority: "High",
                    effortEstimation: "30 hours",
                    notes: "Integration should support various digital book formats."
                );

                var requirement2 = new ProjectRequirement(
                    2,
                    title: "User Account Management",
                    description: "Develop a user management system for library members, including account creation and borrowing history.",
                    type: RequirementType.NonFunctional,
                    status: RequirementStatus.InProgress,
                    priority: "Medium",
                    effortEstimation: "40 hours",
                    notes: "Ensure privacy and security of user data."
                );

                // Create new risks
                var risk1 = new Risk(
                    projectid: 2,
                    riskid: 201,
                    riskname: "Data Migration Challenges",
                    riskdescription: "Potential issues in migrating existing library data to the new system.",
                    riskseverity: 4,
                    riskstatus: true,
                    riskmitigation: "Plan a phased migration with thorough testing at each stage."
                );

                // Create new manhours
                var manhours1 = new LoggedManHours(2, 2, 4, DateTime.Now.AddDays(-7), 2, "Design");
                var manhours2 = new LoggedManHours(2, 4, 6, DateTime.Now.AddDays(-6), 3, "Development");

                var projectPhaseHours2 = new ProjectPhaseHours(2, 2, 4, 3, 2, 1);

                // Create new members and assign them
                var teamMember1 = new ProjectTeamMember(projectid: 2, teammemberid: 4, teammembername: "Alice Johnson", permissionlevel: 3, permissionlevelname: "Project Owner");
                var teamMember2 = new ProjectTeamMember(projectid: 2, teammemberid: 5, teammembername: "Emily White", permissionlevel: 2, permissionlevelname: "Project Manager");

                var user1 = new TeamMember("Alice Johnson");
                var user2 = new TeamMember("Emily White");

                // Reuse members from CreateTestData
                var teamMember3 = new ProjectTeamMember(projectid: 2, teammemberid: 1, teammembername: "John Doe", permissionlevel: 1, permissionlevelname: "Team Member");

                // Save all to database (continues with database operations as in CreateTestData)
                //Save all to database
                project2 = await _dbController.AddProject(project2);
                requirement1 = await _dbController.AddProjectRequirement(requirement1);
                requirement2 = await _dbController.AddProjectRequirement(requirement2);

                risk1 = await _dbController.AddRisk(risk1);

                manhours1 = await _dbController.AddLoggedManHours(manhours1);
                manhours2 = await _dbController.AddLoggedManHours(manhours2);

                projectPhaseHours2 = await _dbController.AddProjectPhaseHours(projectPhaseHours2);

                teamMember1 = await _dbController.AddProjectTeamMember(teamMember1);
                teamMember2 = await _dbController.AddProjectTeamMember(teamMember2);
                teamMember3 = await _dbController.AddProjectTeamMember(teamMember3);

                user1 = await _dbController.AddTeamMember(user1);
                user2 = await _dbController.AddTeamMember(user2);


                project2.ProjectOwnerID = user1.MemberID;

                //update project with new owner
                project2 = await _dbController.UpdateProject(project2);

            }
            catch (Exception e)
            {
                // Exception handling
                Console.WriteLine("There was an issue creating test data.");
                Debug.WriteLine("There was an issue creating test data.");
                return false;
            }
            return true;
        }

        private async Task<bool> CreateTestData3()
        {
            try
            {
                // Create new project
                var project3 = new Project(
                    name: "Employee Performance Tracking System",
                    description: "A system to track and analyze employee performance metrics in a corporate setting.",
                    projectOwnerID: 3,
                    projectOwnerName: "Bob Smith",
                    projectPhase: 1,
                    projectStartDate: DateTime.Now.AddDays(-20)
                );

                // Create new requirements
                var requirement1 = new ProjectRequirement(
                    3,
                    title: "Performance Metrics Dashboard",
                    description: "Create a dashboard to display various performance metrics of employees in real-time.",
                    type: RequirementType.Functional,
                    status: RequirementStatus.Proposed,
                    priority: "High",
                    effortEstimation: "35 hours",
                    notes: "Dashboard should be customizable per department."
                );

                // Create new risks
                var risk1 = new Risk(
                    projectid: 3,
                    riskid: 301,
                    riskname: "Data Privacy Concerns",
                    riskdescription: "Handling sensitive employee data raises privacy and security concerns.",
                    riskseverity: 5,
                    riskstatus: true,
                    riskmitigation: "Implement robust data encryption and access controls."
                );

                // Create new manhours
                var manhours1 = new LoggedManHours(3, 3, 7, DateTime.Now.AddDays(-8), 4, "Design");

                // Create new members and assign them
                var teamMember1 = new ProjectTeamMember(projectid: 3, teammemberid: 6, teammembername: "Bob Smith", permissionlevel: 3, permissionlevelname: "Project Owner");
                var teamMember2 = new ProjectTeamMember(projectid: 3, teammemberid: 7, teammembername: "Sarah Green", permissionlevel: 2, permissionlevelname: "Project Manager");

                // Reuse members from CreateTestData
                var teamMember3 = new ProjectTeamMember(projectid: 3, teammemberid: 2, teammembername: "Jane Doe", permissionlevel: 1, permissionlevelname: "Team Member");

                var projectPhaseHours3 = new ProjectPhaseHours(3, 3, 4, 3, 2, 1);

                var user1 = new TeamMember("Bob Smith");
                var user2 = new TeamMember("Sarah Green");

                // Save all to database (continues with database operations as in CreateTestData)
                //Save all to database
                project3 = await _dbController.AddProject(project3);
                requirement1 = await _dbController.AddProjectRequirement(requirement1);

                risk1 = await _dbController.AddRisk(risk1);

                manhours1 = await _dbController.AddLoggedManHours(manhours1);

                projectPhaseHours3 = await _dbController.AddProjectPhaseHours(projectPhaseHours3);

                teamMember1 = await _dbController.AddProjectTeamMember(teamMember1);
                teamMember2 = await _dbController.AddProjectTeamMember(teamMember2);
                teamMember3 = await _dbController.AddProjectTeamMember(teamMember3);

                user1 = await _dbController.AddTeamMember(user1);
                user2 = await _dbController.AddTeamMember(user2);


                project3.ProjectOwnerID = user1.MemberID;

                //update project with new owner
                project3 = await _dbController.UpdateProject(project3);
            }
            catch (Exception e)
            {
                // Exception handling
                Console.WriteLine("There was an issue creating test data.");
                Debug.WriteLine("There was an issue creating test data.");
                return false;
            }
            return true;
        }

        public async Task UpdateDescription(string description, int projectID)
        {
            var project = this.Projects.Where(x => x.Id == projectID).FirstOrDefault();
            project.Description = description;
            await _dbController.UpdateProject(project);
        }

        public int GetTotalManHours(int empID, int projectID)
        {
            var hours = _dbController.GetLoggedManHoursByProjIDAndEmpID(projectID, empID);
            return hours.Result;
        }

        void RiskReset()
        {
            RiskToInsert = null;
            RiskToUpdate = null;
        }

        public async Task RiskRowUpdate(Risk newRisk)
        {
            await _dbController.UpdateRisk(newRisk);

            //refecth risks
            var risks = await _dbController.GetRisksByProjectID(newRisk.ProjectID);

            //update project
            this.Projects.Where(x => x.Id == newRisk.ProjectID).FirstOrDefault().Risks = risks;

            //refresh grid
            await InvokeAsync(StateHasChanged);
        }

        public async Task RiskEditRow(Risk risk)
        {
            RiskToUpdate = risk;
            await RisksGrid.EditRow(risk);
        }

        public async void OnRiskRowUpdate(Risk risk)
        {
            RiskReset();

            await _dbController.UpdateRisk(risk);
        }

        public async Task RiskDeleteRow(Risk risk)
        {
            await _dbController.DeleteRisk(risk.RiskID);

            //refecth risks
            var risks = await _dbController.GetRisksByProjectID(risk.ProjectID);

            //update project
            this.Projects.Where(x => x.Id == risk.ProjectID).FirstOrDefault().Risks = risks;

            //refresh grid
            await InvokeAsync(StateHasChanged);
        }

        public async Task RiskSaveRow(Risk risk)
        {
            await RisksGrid.UpdateRow(risk);
        }

        public async Task RiskCancelEdit(Risk risk)
        {
            RiskReset();

            RisksGrid.CancelEditRow(risk);
        }

        public void UpdateProjectPhaseHours(int projectID)
        {
            var project = this.Projects.Where(x => x.Id == projectID).FirstOrDefault();
            var projectPhaseHours = project.ProjectPhaseHours;
            _dbController.UpdateProjectPhaseHours(projectPhaseHours);
        }
    }
}