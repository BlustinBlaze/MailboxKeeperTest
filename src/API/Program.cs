using System.Text;
using API;
using Application;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5112");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//For Controllers Support
builder.Services.AddControllers();
builder.Services.AddRouting(r => r.LowercaseUrls = true);
builder.Services.AddSignalR();
//builder.Services.AddHostedService<ServerNotifier>();
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    }));

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
        };
        
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notifications"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                // Log the error or handle it as needed
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

var options = new HiveMQClientOptions();
options.Host = "https://test.mosquitto.org";
options.Port = 8883;
options.UserName = "mailbox";
options.Password = "keeper";
var client = new HiveMQClient(options);
Console.WriteLine($"Connecting to {options.Host}:{options.Port}");

//Connect
HiveMQtt.Client.Results.ConnectResult connectResult;
try
{
    connectResult = await client.ConnectAsync().ConfigureAwait(false);
    if (connectResult.ReasonCode == ConnAckReasonCode.Success)
    {
        Console.WriteLine($"Connect successful: {connectResult}");
    }
    else
    {
        Console.WriteLine($"Connect failed: {connectResult}");
        Environment.Exit(-1);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Connect failed: {ex.Message}");
    Environment.Exit(-1);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints mapping for controllers
app.MapControllers();

app.UseCors("CorsPolicy");
app.MapHub<NotificationHub>("/notifications");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.Run();