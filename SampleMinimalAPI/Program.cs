using System.Text;
using API.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SampleMinimalAPI.Common;
using SampleMinimalAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "basic_or_jwt";
    options.DefaultChallengeScheme = "basic_or_jwt";
})
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null)
            .AddJwtBearer("Bearer", options =>
            {
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = "SampleAPI",
                    ClockSkew = TimeSpan.Zero,
                    ValidAlgorithms = new List<string>() { SecurityAlgorithms.HmacSha256Signature },
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyKey".DecodeBase64())),
                    ValidateLifetime = true
                };
                options.RequireHttpsMetadata = false;
                
            })
            .AddPolicyScheme("basic_or_jwt", "basic_or_jwt", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    string? authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return "Bearer";

                    return "BasicAuthentication";
                };
            });
//inject Mediatr as scoped
builder.Services.AddMediatR(x =>
{
    x.Lifetime = ServiceLifetime.Scoped;
    x.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

//Inject all endpoint definitions
builder.Services.AddEndpointDefinitions(typeof(IEndpointDefinition));

var app = builder.Build();
//Use all endpoint definitions
app.UseEndpointDefinitions();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpointDefinitions();

app.Run();

