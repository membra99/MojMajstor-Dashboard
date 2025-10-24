global using Universal.Util.HtmlHelperExtensions;
using Amazon.S3;
using DotNetEnv;
using Entities.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services;
using Services.Authorization;
using Services.AWS;
using Services.Helpers;
using static Universal.DTO.CommonModels.CommonModels;

var builder = WebApplication.CreateBuilder(args);

//configure dynamic appsetting.json trough env file NOTE: appsetting.json musn't be empty, otherwise this won't work
Env.Load("./.env");
builder.Configuration
    .AddJsonFile($"appsettings.json", optional: false) //if prop from .env file isn't loaded as expected, chances are that ".env" misses . in name or prop key has faulty name
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();

//configure maindatabase
builder.Services.AddDbContext<MainContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainDatabase"));
});

//configure openapi docs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DOT", Version = "v1" });
    c.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//configure cors policy for APIs
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
				policy =>
				{
					policy
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowAnyOrigin();
				}));

// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<MainDataServices>();

//configure aws services
var appSettingsSectionAws = builder.Configuration.GetSection("ServiceConfiguration");
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.Configure<ServiceConfiguration>(appSettingsSectionAws);
builder.Services.AddTransient<IAWSS3FileService, AWSS3FileService>();
builder.Services.AddTransient<IAWSS3BucketHelper, AWSS3BucketHelper>();

// configure for JWT Auth
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<UsersServices>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<Entities.Universal.MainDataNova.MojMajstorContext>(options =>
    options.UseSqlServer("Server=46.38.233.135,1433;Database=MojMajstor;TrustServerCertificate=true;User Id=Sa;Password=YourStrong(!Passw0rd);"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1");
    });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); //Home controller je izbrisan - premeniti ovo
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.UseAuthentication();

app.UseSession();

app.UseMiddleware<JwtMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Index}/{id?}");

app.Run();