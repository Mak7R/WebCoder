using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepositoryStoragesRealizations.DirectoryStorage.RepositoriesStorage;
using RepositoryStoragesRealizations.DirectoryStorage.TempStorage;
using RepositoryStoragesRealizations.FileArchiver;
using WebCoder.Application.Enums;
using WebCoder.Application.Identity;
using WebCoder.Application.Interfaces;
using WebCoder.Application.Options;
using WebCoder.Application.RepositoryInterfaces;
using WebCoder.Application.Services;
using WebCoder.Infrastructure.Data;
using WebCoder.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RepositoryFilesOptions>(builder.Configuration.GetSection("RepositoryFiles"));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 4;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
});

builder.Services.AddScoped<IProjectRepositoriesRepository, ProjectRepositoriesRepository>();
builder.Services.AddScoped<IProjectRepositoriesService, ProjectRepositoriesService>();
builder.Services.AddScoped<IRepositoryFilesService, RepositoryFilesService>();
builder.Services.AddScoped<IRepositoryCommandHandler, RepositoryCommandHandler>();

// start repositories storage
builder.Services.AddSingleton<IFileArchiver, ZipFileArchiver>();
builder.Services.AddSingleton<ITempStorage, TempStorage>(
    _ =>
    {
        var path = builder.Configuration.GetSection("RepositoryFilesOptions")["TempFilesDirectory"] ??
                      throw new Exception("Configuration was not found");
        
        return new TempStorage(path);
    });
builder.Services.AddScoped<IDirectoryRepositoriesStorage, DirectoryRepositoriesStorage>(
    provider =>
    {
        var path = builder.Configuration.GetSection("RepositoryFilesOptions")["WorkingDirectory"] ??
                   throw new Exception("Configuration was not found");
        
        return new DirectoryRepositoriesStorage(path,
            provider.GetRequiredService<ITempStorage>(),
            provider.GetRequiredService<IFileArchiver>()
        );
    }); 
// end repositories storage

var app = builder.Build();

{
    // migrate DB
    using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    // ensure roles

    var rolesManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var roleNames = Enum.GetNames(typeof(Role));
    foreach (var roleName in roleNames)
    {
        if (await rolesManager.FindByNameAsync(roleName) is null)
        {
            var role = new ApplicationRole{ Name = roleName };
            await rolesManager.CreateAsync(role);
        }
    }
}


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();