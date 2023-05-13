using DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAppMVC.Config;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<BookReservationDbContext>(options =>
{
	if (builder.Configuration["Production"] == "0")
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("BookReservationDb")).UseLazyLoadingProxies();
	}
	else
	{
		options.UseMySql(builder.Configuration.GetConnectionString("Production"),
		   ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Production"))).UseLazyLoadingProxies();
	}
});

builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<BookReservationDbContext>();

DiConfig.ConfigureDi(builder.Services, builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
		.AddCookie(o =>
		{
			o.LoginPath = new PathString("/Auth/Login");
			o.AccessDeniedPath = new PathString("/MainPage/AceessDenied");
		});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<BookReservationDbContext>();
	context.Database.EnsureDeleted();
	context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/MainPage/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
		name: "default",
		pattern: "{controller=MainPage}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
