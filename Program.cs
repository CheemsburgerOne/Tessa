using Microsoft.AspNetCore.Authentication.Cookies;
using Tessa.Components;
using Tessa.Models;
using Tessa.Persistance.PostgreSQL;
using Tessa.Utilities.Configuration;

namespace Tessa;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddConfiguration();
        
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddDbContext<TessaDbContext>();
        
        builder.Services.AddModelServices();
        
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "TessaAuthScheme";
                options.LoginPath = "/Login";
                // options.AccessDeniedPath = "/Denied";
                options.ExpireTimeSpan = System.TimeSpan.FromMinutes(15);
                options.SlidingExpiration = true;
            });
        

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {   
            scope.ServiceProvider.GetRequiredService<TessaDbContext>().Initialize();
        }
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        

        app.Run();
    }
}