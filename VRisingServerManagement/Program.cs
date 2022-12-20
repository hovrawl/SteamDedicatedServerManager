using System.Text;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.ResponseCompression;
using VRisingServerManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Server Sent Events
builder.Services.AddServerSentEvents();
builder.Services.AddConsoleService(builder.Configuration);
builder.Services.AddServerSentEvents<IConsoleServerSentEventService, ConsoleServerSentEventService>(options =>
{
    options.ReconnectInterval = 5000;
});

builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/event-stream" });
});

// string builder singleton
builder.Services.AddSingleton<StringBuilder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapServerSentEvents<ConsoleServerSentEventService>("/console-sse");
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();