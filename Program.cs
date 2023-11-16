
using TeamProject.Controllers;

using Microsoft.EntityFrameworkCore;
using Radzen;


var builder = WebApplication.CreateBuilder(args);

//Check if database file exists, if not create it
if (!System.IO.File.Exists("./Database/TeamProjDB.db3"))
{
    System.IO.Directory.CreateDirectory("./Database");
    System.IO.File.Create("./Database/TeamProjDB.db3");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<DialogService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Database/TeamProjDB.db3"));

builder.Services.AddScoped<DatabaseController>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
