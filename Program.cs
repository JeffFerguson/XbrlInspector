using XbrlInspector.Components;
using JeffFerguson.Gepsio;
using Radzen;
using XbrlInspector.ObjectIdMap;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingValue(sp => new XbrlDocument());
builder.Services.AddCascadingValue(sp => new XbrlFragmentMap());
builder.Services.AddCascadingValue(sp => new XbrlSchemaMap());
builder.Services.AddRadzenComponents();

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
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
