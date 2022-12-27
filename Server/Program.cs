using Craftorio.Server.Controllers;
using Craftorio.Shared;
using System.IO;
using Microsoft.AspNetCore.ResponseCompression;

//check for saved games folder
if (!Directory.GetDirectories(Directory.GetCurrentDirectory()).Contains("SavedGames"))
{
    //folder doesnt exist, create one
    Directory.CreateDirectory("./SavedGames/");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ISessionController, SessionController>();
builder.Services.AddSingleton<IPlayerController, PlayerController>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
