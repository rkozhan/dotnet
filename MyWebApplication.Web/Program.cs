using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWebApplication.Web;
using MyWebApplication.Web.Data;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyWebApplicationWebContextConnection") ?? throw new InvalidOperationException("Connection string 'MyWebApplicationWebContextConnection' not found.");

//to get this you should add to .web new scaffolded Item - entity 
//then tools/NuGet package manager/console: Add-Migration Initial //and then; Update-Database
builder.Services.AddDbContext<MyWebApplicationWebContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<MyWebApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MyWebApplicationWebContext>();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("V€r¥ $ecret (not!)|V€r¥ $ecret (not!)|V€r¥ $ecret (not!)"));
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = key
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
