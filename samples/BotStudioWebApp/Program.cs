var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<BotStudioWebApp.Components.App>().AddInteractiveServerRenderMode();
app.Run();
