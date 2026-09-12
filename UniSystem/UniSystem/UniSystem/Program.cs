using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Προσθήκη της σύνδεσης (Connection String) για τη Βάση Δεδομένων
builder.Services.AddDbContext<UniSystem.Models.UniversityGradesDbContext>(options =>
    options.UseSqlServer("Server=Stefanos_Arn\\SQLEXPRESS01;Database=UniversityGradesDB;Trusted_Connection=True;TrustServerCertificate=True;"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(); // Ενεργοποίηση Session

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.UseRouting();
app.UseSession(); // Χρήση Session

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
