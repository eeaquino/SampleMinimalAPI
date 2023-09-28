using SampleMinimalAPI.Common;
using SampleMinimalAPI.Test;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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


app.Run();

