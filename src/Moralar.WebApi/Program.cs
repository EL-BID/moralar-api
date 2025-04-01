using AutoMapper;
using Hangfire.Mongo;
using Moralar.Domain.Services.Interface;
using Moralar.Domain.Services;
using Moralar.WebApi.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moralar.UtilityFramework.Application.Core.JwtMiddleware;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Moralar.Domain.AutoMapper;
using Microsoft.AspNetCore.HttpOverrides;
using Hangfire;
using Hangfire.Console;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Mongo.Migration.Strategies;
using UtilityFramework.Infra.Core.MongoDb.Data.Database;
using SixLabors.ImageSharp.Web.DependencyInjection;
using OfficeOpenXml;
using Microsoft.Extensions.FileProviders;
using Moralar.WebApi.Filter;
using Moralar.WebApi;
using Moralar.WebApi.HangFire.Interface;
using Hangfire.Storage;
using Moralar.WebApi.HangFire;
using ImageResizer.AspNetCore.Helpers;
using Newtonsoft.Json.Converters;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile($"appsettings.json", reloadOnChange: true, optional: false);
builder.Configuration.AddJsonFile($"appsettings.Development.json", optional: true);
builder.Configuration.AddEnvironmentVariables();



/*HANGFIRE*/
BaseSettings settingsDataBase = AppSettingsBase.GetSettings();

var mongoClient = AppSettingsBase.GetMongoClient(settingsDataBase);

var migrationOptions = new MongoMigrationOptions
{
    MigrationStrategy = new MigrateMongoMigrationStrategy(),
    BackupStrategy = new CollectionMongoBackupStrategy()
};
var storageOptions = new MongoStorageOptions
{
    MigrationOptions = migrationOptions,
    Prefix = "HangFire",
    CheckConnection = false
};

builder.WebHost.UseUrls("http://0.0.0.0:8080") /*CASO NECESSARIO COMPARTILHAR LOCALHOST*/
    .UseContentRoot(Directory.GetCurrentDirectory())
    .UseIISIntegration()
    .UseSetting(WebHostDefaults.DetailedErrorsKey, "true");

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers(opt =>
{
    //opt.Filters.Add(typeof(CheckJson));
    opt.Filters.Add(typeof(PreventSpanFilter));
    opt.Filters.Add(typeof(CheckCurrentDevice));
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Moralar WebApi",
            Version = "v1"
        }
     );

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ejm: {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });

    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
});

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;


builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));



builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddMemoryCache();

/*TRANSLATE I18N*/
//services.AddLocalization(options => options.ResourcesPath = "Resources");
//services.AddMvc()
//       .AddDataAnnotationsLocalization(options =>
//       options.DataAnnotationLocalizerProvider = (type, factory) =>
//       factory.Create(typeof(SharedResource)));

//services.Configure<RequestLocalizationOptions>(options =>
//{
//    options.DefaultRequestCulture = new RequestCulture("pt");
//    options.SupportedCultures = SupportedCultures;
//    options.SupportedUICultures = SupportedCultures;
//});

bool EnableSwagger = configuration.GetSection("EnableSwagger").Get<bool>();
bool EnableService = configuration.GetSection("EnableService").Get<bool>();


builder.Services.AddHangfire(configuration =>
{
    configuration.UseConsole();
    configuration.UseMongoStorage(mongoClient, settingsDataBase.Name, storageOptions);
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigin",
        builder => 
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        //.AllowCredentials()
        .Build());
});

IMapper mapper = AutoMapperConfig.RegisterMappings().CreateMapper();
builder.Services.AddSingleton(mapper);


/*CROP IMAGE*/
builder.Services.AddImageSharp(options =>
{
    options.Configuration = Configuration.Default;
    options.BrowserMaxAge = TimeSpan.FromDays(7);
    options.CacheMaxAge = TimeSpan.FromDays(365);
});



/*INJEÇÃO DE DEPENDENCIAS DE BANCO*/
builder.Services.AddRepositoryInjection();

/*INJEÇÃO DE DEPENDENCIAS DE SERVIÇOS*/
builder.Services.AddServicesInjection();

builder.Services.AddResponseCompression();



builder.Services
  .AddAuthentication(options =>
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
          ValidIssuer = "Moralar",
          ValidAudience = "Moralar",
          IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.ASCII.GetBytes("ebbabd773c23a7e55415c64cb40b2f92"))
      };
  });


builder.Services.AddAuthorization();

builder.Services.AddScoped<IUtilService, UtilService>();
var app = builder.Build();

/* PARA USO DO HANG FIRE ROTINAS*/
if (EnableService)
{
    GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(builder.Services.BuildServiceProvider()));
    GlobalJobFilters.Filters.Add(new ProlongExpirationTimeAttribute());

    app.UseHangfireServer();
    app.UseHangfireDashboard("/jobs", new DashboardOptions
    {
        Authorization = new[] { new MyAuthorizationFilter() }
    });
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


/*CROP IMAGE*/
app.UseImageResizer();

app.UseStaticFiles(); // Default static files from wwwroot

app.UseStaticFiles(new StaticFileOptions
{
FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Content")),
RequestPath = "/Content"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "ExportFiles")),
    RequestPath = "/ExportFiles"
});

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
             "Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
    }
});

app.UseRouting();

app.UseMiddleware<TokenProviderMiddleware>();

app.UseCors("AllowAllOrigin");

app.UseResponseCompression();

app.UseAuthentication();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();


String ApplicationName = Assembly.GetEntryAssembly().GetName().Name?.Split('.')[0];

app.UseForwardedHeaders();
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
//}

app.UseHsts();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"../swagger/v1/swagger.json".Trim(), $"API {ApplicationName} {app.Environment.EnvironmentName}");
    c.RoutePrefix = "swagger";

    c.EnableDeepLinking();
    c.EnableFilter();
});

app.MapControllers();

if (EnableService)
{
    using (var connection = JobStorage.Current.GetConnection())
        foreach (var recurringJob in connection.GetRecurringJobs())
            RecurringJob.RemoveIfExists(recurringJob.Id);

    var timeZoneBrazil = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    /*RODA A CADA 30 MINUTO*/
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
    RecurringJob.AddOrUpdate<IHangFireService>(
        "MAKEQUESTIONAVAILABLE",
        services => services.MakeQuestionAvailable(null),
        Cron.MinuteInterval(30), timeZoneBrazil);

    /*RODA TODO DIA AS 9 AM*/
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
    RecurringJob.AddOrUpdate<IHangFireService>(
        "ALERTA_AGENDAMENTO",
        services => services.ScheduleAlert(null),
        Cron.Daily(9), timeZoneBrazil);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                              //Cron.MinuteInterval(5), timeZoneBrazil);

}

app.Run();