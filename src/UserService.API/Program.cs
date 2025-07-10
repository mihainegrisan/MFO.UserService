using Microsoft.EntityFrameworkCore;
using UserService.Application.CommandsQueries.Queries;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options 
    => options.UseSqlServer(builder.Configuration.GetConnectionString("UserContext")));


builder.Services.AddScoped<IUserRepository, UserRepository>();

// This points to the assembly where the MediatR handlers are located.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetUserByIdHandler).Assembly));

//builder.Services.AddDbContext<AppDbContext>(opt =>
//    opt.UseInMemoryDatabase("UserContext"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    //context.Database.EnsureCreated(); // checks if the database exists and creates it with the current model if it doesn't — without using migrations. This creates all the tables directly.
    DbInitializer.Initialize(context);
}

app.MapControllers();

app.Run();
