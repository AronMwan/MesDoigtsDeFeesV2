using Microsoft.EntityFrameworkCore;
using MesDoigtsDeFees.Data;
using Microsoft.AspNetCore.Identity;
using MesDoigtsDeFees.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.OpenApi.Models;
//using MesDoigtsDeFees.Services;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using MesDoigtsDeFees.Services;
//using NETCore.MailKit.Infrastructure.Internal;







var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MesDoigtsDeFeesContext");

builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'MesDoigtsDeFeesContext' not found.")));

builder.Services.AddDefaultIdentity<MesDoigtsDeFeesUser>((IdentityOptions options) => options.SignIn.RequireConfirmedAccount = false)
   .AddRoles<IdentityRole>()
   .AddEntityFrameworkStores<MyDBContext>();

// Add services for globalization/localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Translations");
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

//builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();

//builder.Services.Configure<MailKitOptions>(options =>
//{
//    options.Server = builder.Configuration["ExternalProviders:MailKit:SMTP:Address"];
//    options.Port = Convert.ToInt32(builder.Configuration["ExternalProviders:MailKit:SMTP:Port"]);
//    options.Account = builder.Configuration["ExternalProviders:MailKit:SMTP:Account"];
//    options.Password = builder.Configuration["ExternalProviders:MailKit:SMTP:Password"];
//    options.SenderEmail = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
//    options.SenderName = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderName"];
//    options.Security = true;  
//});

// Add services for RESTFull API
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "MesDoigtsDeFees", Version = "v1" });
});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else 
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MesDoigtsDeFees v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    MyDBContext context = new MyDBContext(services.GetRequiredService<DbContextOptions<MyDBContext>>());
    var userManager = services.GetRequiredService<UserManager<MesDoigtsDeFeesUser>>();
    await MyDBContext.DataInitializer(context, userManager);
}

var supportedCultures = new[] { "en", "nl", "fr" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[1])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//app.UseMiddleware<UserStats>();

app.UseEndpoints(endpoints => 
{ 
    endpoints.MapControllers(); 
});

app.Run();
