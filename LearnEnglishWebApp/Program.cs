using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Implementations;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.Services.Classes;
using LearnEnglishWebApp.Services.Implementations;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Настройка JWT
builder.Services.Configure<JWTSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Пробуем получить токен из cookie
            var token = context.Request.Cookies["auth_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Подключение к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));

//тут регать сервисы и репозитории


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();
builder.Services.AddScoped<IUserWordsRepository, UserWordsRepository>();
builder.Services.AddScoped<IGrammarTopicRepository, GrammarTopicRepository>();
builder.Services.AddScoped<IUserLessonRepository, UserLessonRepository>();
builder.Services.AddScoped<IVocabTopicRepository, VocabTopicRepository>();
builder.Services.AddScoped<IGrammarTestRepository, GrammarTestRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddScoped<IUserWordsService, UserWordsService>();
builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<IGrammarTopicService, GrammarTopicService>();
builder.Services.AddScoped<IUserLessonService, UserLessonService>();
builder.Services.AddScoped<IVocabTopicService, VocabTopicService>();
builder.Services.AddScoped<IGrammarTestService, GrammarTestService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    // Сохраняем оригинальный Response Body
    var originalBodyStream = context.Response.Body;

    using var memoryStream = new MemoryStream();
    context.Response.Body = memoryStream;

    await next();

    // Проверяем, есть ли в ответе токен (из API логина)
    if (context.Response.StatusCode == 200 &&
        context.Request.Path.StartsWithSegments("/api/Auth/login"))
    {
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        // Парсим JSON ответа
        try
        {
            var json = System.Text.Json.JsonDocument.Parse(responseBody);
            if (json.RootElement.TryGetProperty("token", out var tokenElement))
            {
                var token = tokenElement.GetString();
                if (!string.IsNullOrEmpty(token))
                {
                    // Устанавливаем cookie с токеном
                    context.Response.Cookies.Append("auth_token", token, new CookieOptions
                    {
                        HttpOnly = true, // Защита от XSS
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddHours(24)
                    });
                }
            }
        }
        catch { }

        // Возвращаем оригинальный ответ
        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBodyStream);
    }
    else
    {
        // Просто копируем ответ
        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBodyStream);
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        Console.WriteLine($"Serving file: {ctx.File.Name}");
    }
});

// Configure the HTTP request pipeline.
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

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Маршрутизация для MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Инициализация тестовых данных
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