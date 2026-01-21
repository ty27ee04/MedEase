using HealthTech.Components;
using HealthTech.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register HttpClient for API calls with timeout configuration
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5); // Prevent long waits when API is down
});

// Register custom services
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ApiService>();

// Register background service for appointment reminders
builder.Services.AddHostedService<AppointmentReminderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
