using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebTerriane.Data;
using WebTerriane.Models;
using WebTerriane.Services;
using WebTerriane.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Identity: Administrators only. No student registration endpoint
// exists anywhere in the app, so there is nothing to lock down further here
// beyond disabling self-registration UI in the Areas/Admin/Account controller. ---
builder.Services.AddIdentity<Administrator, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 10;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.SignIn.RequireConfirmedEmail = false; // admins are provisioned, not self-signed-up
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/admin/account/login";
    options.AccessDeniedPath = "/admin/account/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// --- File storage: provider switches on config, not on environment name,
// so a client can be pinned to local storage in a pinch without a code
// change. Production deployments should always set Storage:Provider = "Azure". ---
var storageProvider = builder.Configuration["Storage:Provider"] ?? "Azure";
var azureConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");

if (storageProvider.Equals("Azure", StringComparison.OrdinalIgnoreCase)
    && !string.IsNullOrWhiteSpace(azureConnectionString)
    && !azureConnectionString.StartsWith("@Microsoft.KeyVault(", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        var blobServiceClient = new BlobServiceClient(azureConnectionString);
        builder.Services.AddSingleton(blobServiceClient);
        builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
    }
    catch
    {
        builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
    }
}
else
{
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
}

builder.Services.AddScoped<IReferenceNumberService, ReferenceNumberService>();

// --- MVC ---
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
