using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleDotnetApp.Models.DataModels;
using SampleDotnetApp.Services;
using SampleDotnetApp.Utilities;
using SampleDotnetApp.Utilities.Interfaces;
using SampleDotnetApp.Utilities.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options => {
    ApplicationSettings applicationSettings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();
    options.ClientId = applicationSettings.AuthenticationSettings.GoogleAuthenticationSettings.ClientId;
    options.ClientSecret = applicationSettings.AuthenticationSettings.GoogleAuthenticationSettings.ClientSecret;
    options.CallbackPath = "/signin-google";
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.Events.OnCreatingTicket = async ctx => {
        using var scope = ctx.HttpContext.RequestServices.CreateScope();
        var email = ctx.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
        var name = ctx.Principal.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;

        var cacheFactory = scope.ServiceProvider.GetRequiredService<ICacheFactory<DataCacheDataModel>>();
        var dataCacheService = cacheFactory.GetDefaultCache();
        if(!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(name))
        {

            var dataCacheDataModel = new DataCacheDataModel
            {
                Email = email,
                FullName = name,
                HashedPassword = string.Empty
            };
            if(dataCacheService.AddToCache(dataCacheDataModel))
            {
                ctx.Principal.AddIdentity(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim("FullName", name)
                }, "Google"));
            }
        }
    };
});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<BasicDataCacheService>();
builder.Services.AddSingleton<RTWTCacheService>();
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ApplicationSettings>>().Value);
builder.Services.AddSingleton<ICacheFactory<DataCacheDataModel>, CacheFactory>();
builder.Services.AddDbContext<ApplicationDBContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("SQLConnectionString")));

var app = builder.Build();
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("api/minimal", () => "Minimal API is working!");
app.MapPost("api/minimal", async (HttpContext httpContext) => 
{
    try
    {
        SampleAPIDataModel? sampleAPIDataModel = await httpContext.Request.ReadFromJsonAsync<SampleAPIDataModel>();
        if(sampleAPIDataModel == null || !string.IsNullOrWhiteSpace(sampleAPIDataModel.Data))
        {
            await httpContext.Response.WriteAsJsonAsync(new { ErrorMessage = "Error : Invalid Data!", ErrorCode = 400 });
        }
        else
        {
            await httpContext.Response.WriteAsJsonAsync(new { Success = true, Message = "Minimal API is working!", Data = sampleAPIDataModel.Data });
        }
    }
    catch (Exception ex)
    {
        throw new ArgumentException(nameof(ex));
    }
});
app.MapControllerRoute(name : "default", pattern : "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
