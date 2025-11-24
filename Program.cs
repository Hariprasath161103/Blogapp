using Blazored.LocalStorage;
using Blogapp.Components;
using Blogapp.Services;
using Blogapp.Hubs; // Added for CommentHub

var builder = WebApplication.CreateBuilder(args);

// Add SignalR services
builder.Services.AddSignalR();

builder.Services.AddScoped<SupabaseService>();
builder.Services.AddSingleton<MongoBlogService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredLocalStorage(config =>
{
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Inside Program.cs

builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        // Increase limit to 10 MB (default is 32 KB)
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map the SignalR Hub endpoint
app.MapHub<CommentHub>("/commentHub");

app.Run();
