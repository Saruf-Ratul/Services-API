using AspNetCoreRateLimit;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Services.API.Filters;
using Services.API.Middleware;
using Services.Application.Common.Mappings;
using Services.Infrastructure.Data;
using Services.Infrastructure.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using HealthChecks.UI.Client;
using SoapCore;
using MediatR;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddEndpointsApiExplorer();

// Advanced Swagger Configuration with Security
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Services API",
        Version = "v1",
        Description = "Clean Architecture SOAP/REST API with OAuth2.1/OIDC and MFA support",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@servicesapi.com"
        },
        License = new OpenApiLicense
        {
            Name = "Proprietary",
            Url = new Uri("https://servicesapi.com/license")
        }
    });

    // Add XML Documentation
    try
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }
    catch
    {
        // Ignore if XML comments file not found
    }

    // JWT Bearer Security Definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below. 
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // OAuth2/OIDC Security Definition (optional - only if configured)
    try
    {
        var authority = builder.Configuration["OidcSettings:Authority"];
        if (!string.IsNullOrEmpty(authority) && !authority.Contains("your-identity-provider") && Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri))
        {
            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorityUri, "/authorize"),
                        TokenUrl = new Uri(authorityUri, "/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect" },
                            { "profile", "User Profile" },
                            { "email", "Email Access" },
                            { "offline_access", "Offline Access" },
                            { "api", "API Access" }
                        }
                    }
                }
            });
        }
    }
    catch
    {
        // OAuth2 configuration failed, skip it
    }

    // Basic Authentication Security Definition
    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Basic Authentication header"
    });

    // Apply security globally with filters (commented out if causing issues)
    // options.OperationFilter<SecurityRequirementsOperationFilter>();
    // options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

    // Add bearer token examples
    // options.ExampleFilters();

    // Enable API versioning in Swagger
    options.EnableAnnotations();

    // Custom schema IDs
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

// Database with retry logic
builder.Services.AddDbContext<Services.Infrastructure.Data.ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")!,
    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null)
    ), ServiceLifetime.Scoped);

// Redis Cache - Commented out until Redis is installed
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration.GetConnectionString("Redis");
//     options.InstanceName = "Services:";
// });

// builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// MediatR
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly);
});

// Register validation behavior as pipeline behavior
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(MappingProfile).Assembly);

// CORS with production-ready configuration
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
    ?? new[] { "*" };

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("Development", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("X-Pagination", "X-Rate-Limit-Remaining");
        });
    }
    else
    {
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Pagination", "X-Rate-Limit-Remaining")
                  .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        });
    }
});

// OAuth2.1/OIDC Authentication with MFA support
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var oidcSettings = builder.Configuration.GetSection("OidcSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured"))),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("Token validated for user: {User}", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
})
.AddOpenIdConnect(options =>
{
    options.Authority = oidcSettings["Authority"];
    options.ClientId = oidcSettings["ClientId"];
    options.ClientSecret = oidcSettings["ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("offline_access");
    options.Scope.Add("api");
    
    // MFA Support
    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            var mfaRequired = context.Principal?.Claims.FirstOrDefault(c => c.Type == "amr" && c.Value == "mfa");
            if (mfaRequired != null)
            {
                Log.Information("MFA required for user: {User}", context.Principal?.Identity?.Name);
                context.Properties.Items["mfa_required"] = "true";
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireMFA", policy => 
    {
        policy.RequireClaim("amr", "mfa");
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy("AdminOnly", policy => 
    {
        policy.RequireRole("Admin");
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy("UserOrAdmin", policy => 
    {
        policy.RequireRole("User", "Admin");
        policy.RequireAuthenticatedUser();
    });
});

// Health Checks - Basic check only (database checks disabled for now)
builder.Services.AddHealthChecks();

builder.Services.AddHealthChecksUI(options =>
{
    options.AddHealthCheckEndpoint("Services API", "/health");
}).AddInMemoryStorage();

// SOAP Support
builder.Services.AddSoapCore();

var app = builder.Build();

// Global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Services API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Services API - Development";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.EnableTryItOutByDefault();
        options.EnablePersistAuthorization();
        options.OAuthClientId(builder.Configuration["OidcSettings:ClientId"]);
        options.OAuthClientSecret(builder.Configuration["OidcSettings:ClientSecret"]);
        options.OAuthScopes("openid", "profile", "email", "api");
        options.OAuthUsePkce();
    });
}
else if (builder.Configuration.GetValue<bool>("SwaggerSettings:EnableSwaggerUI"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Configuration["SwaggerSettings:DocumentTitle"] ?? "Services API");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = builder.Configuration["SwaggerSettings:DocumentTitle"] ?? "Services API";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        if (builder.Configuration.GetValue<bool>("SwaggerSettings:RequireAuthentication"))
        {
            options.EnablePersistAuthorization();
            options.OAuthClientId(builder.Configuration["OidcSettings:ClientId"]);
            options.OAuthClientSecret(builder.Configuration["OidcSettings:ClientSecret"]);
        }
    });
}

// Security Headers
if (builder.Configuration.GetValue<bool>("SecuritySettings:EnableSecurityHeaders", true))
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
        
        if (builder.Configuration.GetValue<bool>("SecuritySettings:EnableHSTS", true))
        {
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
        }
        
        await next();
    });
}

app.UseHsts();
app.UseHttpsRedirection();

// Response Compression
app.UseResponseCompression();

// Rate Limiting
app.UseIpRateLimiting();

// CORS
app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

// SOAP Endpoint - Register your SOAP service here
// app.UseSoapEndpoint<YourSoapService>("/DeviceService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (_, elapsed, ex) => ex != null
        ? Serilog.Events.LogEventLevel.Error
        : elapsed > 500
            ? Serilog.Events.LogEventLevel.Warning
            : Serilog.Events.LogEventLevel.Information;
});

Log.Information("Starting Services API");

app.Run();
