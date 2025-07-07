using Congratulate.Infrastructure.Data;
using Congratulate.Infrastructure.Interfaces;
using Congratulate.Infrastructure.Repositories;
using Congratulate.Application.Interfaces;
using Congratulate.Application.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BirthdayContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService>(provider =>
{
    var repo = provider.GetRequiredService<IPersonRepository>();
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var photoDir = Path.Combine(env.ContentRootPath, "Photos");
    if (!Directory.Exists(photoDir)) Directory.CreateDirectory(photoDir);
    return new PersonService(repo, photoDir);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
