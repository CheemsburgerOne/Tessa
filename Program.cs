using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Tessa.Components;
using Tessa.Models;
using Tessa.Persistance.PostgreSQL;
using Tessa.Utilities;
using Tessa.Utilities.Configuration;

namespace Tessa;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddUtilities();
        
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddControllers();

        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddDbContext<TessaDbContext>();
        
        builder.Services.AddModelServices();
        
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "TessaAuthScheme";
                options.LoginPath = "/login";
                // options.AccessDeniedPath = "/Denied";
                options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
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
        // using (var scope = app.Services.CreateScope())
        // {
        //     string root = scope.ServiceProvider.GetRequiredService<IConfigurationService>().Configuration.Root;
        //     app.UseStaticFiles(new StaticFileOptions()
        //     {
        //         FileProvider = new PhysicalFileProvider(root),
        //         RequestPath = "/download-request"
        //     });
        // }
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        app.MapControllerRoute("default", "{controller}/{action}/{data}");

        app.Run();
    }
}