using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Implementations;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.Services.Implementations;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/Home/Index";
        options.LogoutPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/Index";
        options.ExpireTimeSpan = TimeSpan.FromDays(7); 
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins("https://localhost:7293", "http://localhost:5083")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Подключение к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));

// Регистрация сервисов
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();
builder.Services.AddScoped<IUserWordsRepository, UserWordsRepository>();
builder.Services.AddScoped<IGrammarTopicRepository, GrammarTopicRepository>();
builder.Services.AddScoped<IUserLessonRepository, UserLessonRepository>();
builder.Services.AddScoped<IVocabTopicRepository, VocabTopicRepository>();
builder.Services.AddScoped<IGrammarTestRepository, GrammarTestRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddScoped<IUserWordsService, UserWordsService>();
builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<IGrammarTopicService, GrammarTopicService>();
builder.Services.AddScoped<IUserLessonService, UserLessonService>();
builder.Services.AddScoped<IVocabTopicService, VocabTopicService>();
builder.Services.AddScoped<IGrammarTestService, GrammarTestService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}


app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowSpecific");

app.Use(async (context, next) =>
{
    Console.WriteLine($">>> REQUEST: {context.Request.Method} {context.Request.Path}");

    await next();

    Console.WriteLine($"<<< RESPONSE: {context.Response.StatusCode} for {context.Request.Path}");

    if (context.Response.StatusCode == 302 || context.Response.StatusCode == 301)
    {
        var location = context.Response.Headers["Location"].ToString();
        Console.WriteLine($"!!! REDIRECT to: {location}");
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Применяем миграции...");
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        logger.LogInformation("Заполняем тестовыми данными...");
        var seedService = services.GetRequiredService<ISeedService>();
        await seedService.SeedAllDataAsync();

        logger.LogInformation("Готово!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка при инициализации БД");
    }
}

app.Run();