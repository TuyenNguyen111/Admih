using FinalProject.Controllers.API;
using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Middleware;
using FinalProject.Services;
using FinalProject.Services.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddControllers();


builder.Services.AddDbContext<QlptContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhongTro"));
});


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IEmailSender, EmailService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Customer/SignIn";
    options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FavoriteListService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<RoomFeedbackService>();
builder.Services.AddScoped<ResponseListService>();
builder.Services.AddScoped<PrivilegeService>();
builder.Services.AddScoped<AccessManagementService>();

builder.Services.AddScoped<UserManagementService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:7127") 
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); 
        });
});


builder.Services.AddHttpClient<GeocodeController>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseCors("MyAllowSpecificOrigins");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<PageAccessMiddleware>();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
