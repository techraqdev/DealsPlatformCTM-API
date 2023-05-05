using API;
using API.AppAuthorization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Azure.Identity;
using Deals.Business.Mappers;
using Deals.Business.Validators;
using Common;
using Common.Helpers;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");


try
{
    //Configuration Setting
    builder.Configuration.AddEnvironmentVariables();
    var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    Log.Information($"The env is {envName}");

    //Serilog
    builder.Host.UseSerilog((ctx, lc) =>
    {
        string appInsightsInstrKey = builder.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
        if (!string.IsNullOrWhiteSpace(appInsightsInstrKey))
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = appInsightsInstrKey;
            lc.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
        }
        lc.ReadFrom.Configuration(ctx.Configuration);
    });

    var keyVaultEndpoint = builder.Configuration["KeyVaultUrl"];
    var tenantId = builder.Configuration["AZURE_TENANT_ID"];
    var clientId = builder.Configuration["AZURE_CLIENT_ID"];
    var clientSecret = builder.Configuration["AZURE_CLIENT_SECRET"];
    Azure.Core.TokenCredential cred = new ClientSecretCredential(tenantId, clientId, clientSecret);
    //builder.Configuration.AddAzureKeyVault(new Uri(keyVaultEndpoint), cred);
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultEndpoint), cred);

    //Cors   
    builder.Services.AddCors(options =>
    {
        // this defines a CORS policy called "default"
        options.AddPolicy("default", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    if (!Convert.ToBoolean(builder.Configuration["AppSettings:IsJwtFlow"]))
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(cfg =>
            {
                cfg.Authority = builder.Configuration["JwtConfig:Authority"];
                cfg.Audience = builder.Configuration["JwtConfig:Audience"];
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };
            });

        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //            .AddMicrosoftIdentityWebApi(builder.Configuration)
        //                .EnableTokenAcquisitionToCallDownstreamApi()
        //                    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
        //                    .AddInMemoryTokenCaches();
    }
    else
    {
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            var Key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Key)

            };
        });
    }




    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

    builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddProjectValidator>());

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(conf =>
    {
        //use fully qualified object names
        conf.CustomSchemaIds(x => x.FullName);
        conf.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Deals Platform API",
            Description = "Deals Platform"
        });

        conf.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "\"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        });

        conf.AddSecurityRequirement(new OpenApiSecurityRequirement{ { new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        } },new List<string>()
                        }
                    });
    });


    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddDbContext<DealsPlatformContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DealsPlatformDBCon")));
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

    builder.Services.AddScoped<IClaimsTransformation, AddRolesClaimsTransformation>();
    builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

    // Register the autofac modules
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.IntegrateContainer();
    });

    //Automapper
    builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(ProjectMapper)));

    var app = builder.Build();

    string swaggerEnabled = builder.Configuration["EnableSwagger"];
    if (swaggerEnabled == "true")
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    //app.UseSwagger();
    //app.UseSwaggerUI();

    // string spireLicenceKey = @"C:\Users\schinthala\Downloads\license.elic.xml";
    //if (!string.IsNullOrEmpty(spireLicenceKey))
    //{
    //    //Spire.License.LicenseProvider.SetLicenseKey();
    //    Spire.License.LicenseProvider.SetLicenseFileFullPath(spireLicenceKey);
    //    String fileName = Spire.License.LicenseProvider.GetLicenseFileName();
    Spire.License.LicenseProvider.SetLicenseKey("fbtdbshM05PZAQCxhHw1XBXTTehW0rSkgWNrKAsXamAJK6F2wzPJwKPMV+dVf+R2Uo3xtJUL12WsWGpyd6j1hJw2xX35FlPyHcrXAhgEKGPSsKDqTV5KzGmCSjkFdBsyW1AwqSwZnAYaIueMkBvC37yAg3k/Q58T2zIn+ZIdyHhdsULSljtHIBdcKY7rUL+ZL3jQikfPo//7t0V10IsM7oLXgeqa3rl8Fzlxudi5OkW4M69gvgdKOvXwQoX9Rb/StoIzn7V4LBFLJoPhWzaPOSczhkffu361rbp7KIKEiFNvrhpBSXUZkwK3/X7uWRHPIDVuiL3EC9Ws8jCkQmSvgB/54EOOoSt69F9zRpjlnggKbOM0d8F3GaoCB4CNXVqx1uVJx7gtnylczOHXDB+jDRnnpn2AHynI8IvfRkwXyOJInE5uo4JL3KdwXXRiNpgyBNPGDZME1bYbzna7EsUk+pLEze36E6iHHBJO+W0I0jVfsNx7MSKjE+1BkIjbd0v9Iuf7I5qK+wAsBBQKqQM+cqzeKpR9rxPIrkiZlGa34lz2qrwKvUx4+MQorsWkC+BTL1ZxozGXq8JLDA1NzojR35vO3tgdFoXPxXq5pjeTL+Ljf/gLoJJgYQ80ZgrnJDdTbLe0J2WC/elu0TRb+a677WDygr0rQ7xeduJqPM4OsuczYSxKUNYN4pTGaCsTySQGvkJ+j7nJfbYXwIDAQi+Ne8n80fmhqJQEqxJ5/mreGKR8BQmQ/uekDPDmxO0Hkgciz+RHRP4obgL+Id6WkQsnPdQfoxSavBMNp7MfBEXSlhx7afcCknv2gEiqKlkiIqn232fs1Dh/DGGH2TP4F4oH4CwF2bmFg687JBiWMF7M9v4EkSkscdNQzW2eAno26rnxoL7GHm1Ew9iENwXoargdNOmYcEQWJUt/lVj5QQ7c/dp1c8MSL75zMEAaHgXlV6NPftf+hHhYOj8oTsaWJtUICSmR+FT4+aKR44BvXvgx7yzy77M81J7Zs/VziejlG9KQMOZHfXJ+0USzZCvBOPugoJByJsstYsbSBLorq096tr4adkjCHxPCJSqBVR5Hv99qacRuh05G9ZZiFtOiuSrd68pfKDJTXvlytZ1/hJ7o3IEPf0eeoevlfACSpE5tZegPRBj3lRJ4SDUADxPPuFwMMX4Y+62Yv75prB/uB3o9hIcDV8K689J/mM3rmQHXfFj5kGbc8OMlcIPNqO/jMlBUxhBez93BV7XKBl2e0ovRfJ2BsIY2tjheqEdRyfJY9Z+Da0sQEe0mmWaGrS/zAs3p7saz0sk0b4dFuInHTgw1Ie+/mu+XUoA4YSjFyTtM4BtE3TTQqGb0yvDDsg+7ww4hLeGigrx7FPE3FaZDyAJWtDcVnwEhgkKeKO8Mr0u8ccmOWy8UqjElmPbkCy4uAj68DH53VanWC2Vc6n+cmRXJJ0Z4sON8lMHxPXTj9M6sM/E8oiSRGJyuPH9weEBQ96nOgdFYcaN+HjMVOpo7Q4Z7xrBEK80M45ZlDDtl2UGireedDnDoPFBGQZBfU+EUKqNw3JFt9lE6YN33jze85W+TzZph32avwjQc8+lTrbwCnEo5a1csuZ7IcnMGzQswL+huAUULSpE7peIxLjzPg7jsWMlejFRf2lmj6heZsJkbbzZFVdko7qjhc7k=");
    Spire.License.LicenseProvider.LoadLicense();
    //}
    //else
    //{
    //    Log.Information("SpireLicenseKey is not present");
    //}
    app.UseCors("default");

    //app.UseMiddleware<JwtMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Error($"Error in startup {ex.Message}");
}
finally
{

}