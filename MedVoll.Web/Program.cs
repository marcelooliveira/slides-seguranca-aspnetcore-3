using MedVoll.Web.Filters;
using MedVoll.Web.Interfaces;
using MedVoll.Web.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExceptionHandlerFilter>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ExceptionHandlerFilter>();
});

var httpClientUrl = builder.Configuration["MedVoll.WebApi.Url"];
var httpClientName = builder.Configuration["MedVoll.WebApi.Name"];

builder.Services.AddHttpClient(
    httpClientName,
    client =>
    {
        // Set the base address of the named client.
        client.BaseAddress = new Uri(httpClientUrl);
    });

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<IMedVollApiService, MedVollApiService>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/erro/500");
    app.UseStatusCodePagesWithReExecute("/erro/{0}");
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
