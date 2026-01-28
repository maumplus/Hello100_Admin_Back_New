using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.HealthChecks;
using Hello100Admin.Modules.Auth.Infrastructure;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.BuildingBlocks.Common.Domain;
using Hello100Admin.Modules.Admin.Infrastructure;
using FluentValidation;
using Hello100Admin.Modules.Seller.Infrastructure;
using Hello100Admin.API.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSeller;
using Hello100Admin.Modules.Seller.Application.Features.Bank.Queries.GetBankList;
using Hello100Admin.API.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries.GetAdminUser;
using Hello100Admin.Modules.Admin.Application.Features.Member.Queries.GetMember;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;

// Dapper snake_case <-> PascalCase 자동 매핑 설정
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

// Serilog 설정
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "Hello100Admin")
    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<CustomJwtBearerEvents>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Hello100Admin API",
        Version = "v1",
        Description = "Hello100Admin 관리자 시스템 API"
    });

    // JWT 인증 추가
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS 설정
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication 설정
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        NameClaimType = JwtRegisteredClaimNames.Sub,
        ClockSkew = TimeSpan.Zero
    };

    options.MapInboundClaims = false;
    options.EventsType = typeof(CustomJwtBearerEvents);
});

builder.Services.AddAuthorization();

// MediatR 등록
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetAdminUserQuery).Assembly);
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateSellerCommand).Assembly);  // Add Seller
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetBankListQuery).Assembly);  // Add Seller
});

// MediatR 파이프라인에 ValidationBehavior 등록
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Hello100Admin.BuildingBlocks.Common.Behaviors.ValidationBehavior<,>));

// FluentValidation Validator 자동 등록 (어셈블리 스캔)
builder.Services.AddValidatorsFromAssemblyContaining<GetMemberQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSellerCommandValidator>(); // Add Seller

// Crypto Service 등록 (암호화/복호화)
builder.Services.AddSingleton<ICryptoService, AesCryptoService>();



// HealthCheck용 DB 커넥션 팩토리 싱글턴 등록 (Auth DB 기준)
builder.Services.AddSingleton<IDbConnectionFactory>(
    new Hello100Admin.Modules.Auth.Infrastructure.Persistence.DbConnectionFactory(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection is not configured")
    )
);

// Auth Infrastructure 등록
builder.Services.AddAuthInfrastructure(builder.Configuration);
// Admin Infrastructure 등록
builder.Services.AddAdminInfrastructure(builder.Configuration);
// Seller Infrastructure 등록
builder.Services.AddSellerInfrastructure(builder.Configuration);

// HealthCheck: Dapper/MySQL 연결 체크
builder.Services.AddHealthChecks()
    .AddCheck<MySqlDapperHealthCheck>("mysql-dapper", tags: new[] { "ready", "live" });

var app = builder.Build();

// 글로벌 예외 처리 미들웨어 등록 (항상 최상단)
app.UseMiddleware<Hello100Admin.API.Middleware.ExceptionMiddleware>();

// EncryptedData에 CryptoService 주입 (정적 설정)
var cryptoService = app.Services.GetRequiredService<ICryptoService>();
EncryptedData.Configure(cryptoService);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // ✅ Stoplight Elements
    app.UseStoplight();
}

// Serilog 요청 로깅 추가
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

            // Trace / Correlation Ids for cross-request tracing
            diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
            var correlation = httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationValues)
                && !string.IsNullOrEmpty(correlationValues.ToString())
                ? correlationValues.ToString()
                : httpContext.TraceIdentifier;
            diagnosticContext.Set("CorrelationId", correlation);
    };
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();


// HealthCheck 엔드포인트 매핑
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapControllers();

app.Run();

// Make the implicit Program class public for WebApplicationFactory in integration tests
public partial class Program { }
