using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Hackathon_VAIT_New.Data;
using DotNetEnv;
using Hackathon_VAIT_New.Services;
using Hackathon_VAIT_New.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddBlazorBootstrap();


builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<FirebaseStorageServices>();
builder.Services.AddSingleton<OpenAIService>();

builder.Services.AddScoped(sp => new HttpClient(new AddHeadersDelegatingHandler())
{
    BaseAddress = new Uri("https://api.openai.com/v1/responses")
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Env.Load();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();